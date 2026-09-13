using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class RecepcionCompraService
    : IRecepcionCompraService
{
    private static readonly HashSet<string> TiposRecepcionPermitidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "PARCIAL",
            "TOTAL"
        };

    private readonly IRecepcionCompraRepository _repository;

    public RecepcionCompraService(
        IRecepcionCompraRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<RecepcionCompraResponse>> ListarAsync(
        RecepcionCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (filtro.IdEmpresa is null ||
            filtro.IdEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        filtro.Pagina =
            Math.Max(filtro.Pagina, 1);

        filtro.TamanoPagina =
            Math.Clamp(
                filtro.TamanoPagina,
                1,
                100);

        filtro.Estado =
            NormalizarTexto(filtro.Estado)?
                .ToLowerInvariant();

        filtro.Buscar =
            NormalizarTexto(filtro.Buscar);

        if (filtro.FechaDesde.HasValue &&
            filtro.FechaHasta.HasValue &&
            filtro.FechaHasta.Value.Date <
            filtro.FechaDesde.Value.Date)
        {
            throw new ArgumentException(
                "La fecha final no puede ser anterior a la fecha inicial.");
        }

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<RecepcionCompraDetalleResponse?> ObtenerPorIdAsync(
        long idRecepcionCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idRecepcionCompra <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult<
                RecepcionCompraDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idRecepcionCompra,
            idEmpresa,
            cancellationToken);
    }

    public async Task<(
        long IdRecepcionCompra,
        string NumeroRecepcion)> RegistrarAsync(
        RecepcionCompraCrearRequest request,
        int idEmpresa,
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        if (idEmpleado <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al empleado.");
        }

        if (request.IdOrdenCompra <= 0)
        {
            throw new ArgumentException(
                "La orden de compra es obligatoria.");
        }

        if (request.IdAlmacen <= 0)
        {
            throw new ArgumentException(
                "El almacén es obligatorio.");
        }

        request.TipoRecepcion =
            NormalizarTexto(request.TipoRecepcion)?
                .ToUpperInvariant()
            ?? throw new ArgumentException(
                "El tipo de recepción es obligatorio.");

        request.NumeroGuiaRemision =
            NormalizarTexto(
                request.NumeroGuiaRemision);

        request.Observacion =
            NormalizarTexto(
                request.Observacion);

        if (!TiposRecepcionPermitidos.Contains(
                request.TipoRecepcion))
        {
            throw new ArgumentException(
                "El tipo de recepción debe ser PARCIAL o TOTAL.");
        }

        if (request.FechaRecepcion == default)
        {
            request.FechaRecepcion =
                DateTime.Now;
        }

        if (request.FechaRecepcion.Date >
            DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha de recepción no puede ser futura.");
        }

        if (request.Detalles is null ||
            request.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "La recepción debe contener productos.");
        }

        var duplicados =
            request.Detalles
                .GroupBy(
                    x => x.IdDetalleOrdenCompra)
                .Any(
                    x => x.Count() > 1);

        if (duplicados)
        {
            throw new ArgumentException(
                "Existen detalles de orden repetidos.");
        }

        foreach (var detalle in request.Detalles)
        {
            if (detalle.IdDetalleOrdenCompra <= 0)
            {
                throw new ArgumentException(
                    "Existe un detalle de orden inválido.");
            }

            if (detalle.IdProducto <= 0)
            {
                throw new ArgumentException(
                    "Existe un producto inválido.");
            }

            if (detalle.CantidadEsperada <= 0)
            {
                throw new ArgumentException(
                    "La cantidad esperada debe ser mayor que cero.");
            }

            if (detalle.CantidadRecibida <= 0)
            {
                throw new ArgumentException(
                    "La cantidad recibida debe ser mayor que cero.");
            }

            if (detalle.CantidadAceptada < 0)
            {
                throw new ArgumentException(
                    "La cantidad aceptada no puede ser negativa.");
            }

            if (detalle.CantidadRechazada < 0)
            {
                throw new ArgumentException(
                    "La cantidad rechazada no puede ser negativa.");
            }

            if (detalle.CantidadAceptada +
                detalle.CantidadRechazada !=
                detalle.CantidadRecibida)
            {
                throw new ArgumentException(
                    "La cantidad aceptada más la rechazada debe coincidir con la cantidad recibida.");
            }

            detalle.CondicionProducto =
                NormalizarTexto(
                    detalle.CondicionProducto);

            detalle.Lote =
                NormalizarTexto(
                    detalle.Lote);

            detalle.Observacion =
                NormalizarTexto(
                    detalle.Observacion);

            if (detalle.FechaVencimiento.HasValue &&
                detalle.FechaVencimiento.Value.Date <
                request.FechaRecepcion.Date)
            {
                throw new ArgumentException(
                    "La fecha de vencimiento no puede ser anterior a la recepción.");
            }
        }

        return await _repository.RegistrarAsync(
            request,
            idEmpresa,
            idEmpleado,
            cancellationToken);
    }

    public async Task<bool> AnularAsync(
        long idRecepcionCompra,
        int idEmpresa,
        int idEmpleado,
        RecepcionCompraAnularRequest request,
        CancellationToken cancellationToken)
    {
        if (idRecepcionCompra <= 0 ||
            idEmpresa <= 0)
        {
            return false;
        }

        if (idEmpleado <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al empleado.");
        }

        request.Motivo =
            NormalizarTexto(request.Motivo)
            ?? throw new ArgumentException(
                "El motivo de anulación es obligatorio.");

        var recepcion =
            await _repository.ObtenerPorIdAsync(
                idRecepcionCompra,
                idEmpresa,
                cancellationToken);

        if (recepcion is null)
        {
            return false;
        }

        if (recepcion.Estado.Equals(
                "anulada",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La recepción ya se encuentra anulada.");
        }

        return await _repository.AnularAsync(
            idRecepcionCompra,
            idEmpresa,
            idEmpleado,
            request.Motivo,
            cancellationToken);
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}