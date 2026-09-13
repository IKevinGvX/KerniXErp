using KernixERP.DTOs.Common;
using KernixERP.DTOs.OrdenesCompra.Requests;
using KernixERP.DTOs.OrdenesCompra.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class OrdenCompraService
    : IOrdenCompraService
{
    private static readonly HashSet<string> MonedasPermitidas =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "PEN",
            "USD"
        };

    private readonly IOrdenCompraRepository _repository;
    private readonly ISolicitudCompraRepository _solicitudRepository;

    public OrdenCompraService(
        IOrdenCompraRepository repository,
        ISolicitudCompraRepository solicitudRepository)
    {
        _repository = repository;
        _solicitudRepository = solicitudRepository;
    }

    public Task<PagedResponse<OrdenCompraResponse>> ListarAsync(
        OrdenCompraFiltroRequest filtro,
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

        filtro.Moneda =
            NormalizarTexto(filtro.Moneda)?
                .ToUpperInvariant();

        filtro.Buscar =
            NormalizarTexto(filtro.Buscar);

        if (filtro.Moneda is not null &&
            !MonedasPermitidas.Contains(
                filtro.Moneda))
        {
            throw new ArgumentException(
                "La moneda debe ser PEN o USD.");
        }

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

    public Task<OrdenCompraDetalleResponse?> ObtenerPorIdAsync(
        long idOrdenCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idOrdenCompra <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult<
                OrdenCompraDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idOrdenCompra,
            idEmpresa,
            cancellationToken);
    }

    public async Task<(
        long IdOrdenCompra,
        string NumeroOrden)> GenerarDesdeSolicitudAsync(
        OrdenCompraDesdeSolicitudRequest request,
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

        if (request.IdSolicitudCompra <= 0)
        {
            throw new ArgumentException(
                "La solicitud de compra es obligatoria.");
        }

        if (request.IdProveedor <= 0)
        {
            throw new ArgumentException(
                "El proveedor es obligatorio.");
        }

        Normalizar(request);

        if (!MonedasPermitidas.Contains(
                request.Moneda))
        {
            throw new ArgumentException(
                "La moneda debe ser PEN o USD.");
        }

        if (request.TipoCambio <= 0)
        {
            throw new ArgumentException(
                "El tipo de cambio debe ser mayor que cero.");
        }

        if (request.FechaEntregaEstimada.HasValue &&
            request.FechaEntregaEstimada.Value.Date <
            DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha estimada de entrega no puede ser anterior a hoy.");
        }

        if (request.Detalles is null ||
            request.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "La orden debe contener productos.");
        }

        var duplicados =
            request.Detalles
                .GroupBy(x =>
                    x.IdDetalleSolicitud)
                .Any(x =>
                    x.Count() > 1);

        if (duplicados)
        {
            throw new ArgumentException(
                "Existen detalles repetidos.");
        }

        foreach (var detalle in request.Detalles)
        {
            if (detalle.IdDetalleSolicitud <= 0)
            {
                throw new ArgumentException(
                    "Existe un detalle de solicitud inválido.");
            }

            if (detalle.PrecioUnitario <= 0)
            {
                throw new ArgumentException(
                    "El precio unitario debe ser mayor que cero.");
            }

            if (detalle.Descuento < 0)
            {
                throw new ArgumentException(
                    "El descuento no puede ser negativo.");
            }

            if (detalle.PorcentajeIgv < 0 ||
                detalle.PorcentajeIgv > 100)
            {
                throw new ArgumentException(
                    "El porcentaje de impuesto no es válido.");
            }
        }

        var solicitud =
            await _solicitudRepository.ObtenerPorIdAsync(
                request.IdSolicitudCompra,
                idEmpresa,
                cancellationToken);

        if (solicitud is null)
        {
            throw new KeyNotFoundException(
                "Solicitud de compra no encontrada.");
        }

        if (!solicitud.Estado.Equals(
                "aprobada",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Solo se puede generar una orden desde una solicitud aprobada.");
        }

        return await _repository
            .GenerarDesdeSolicitudAsync(
                request,
                idEmpresa,
                idEmpleado,
                cancellationToken);
    }

    public async Task<bool> AnularAsync(
        long idOrdenCompra,
        int idEmpresa,
        int idEmpleado,
        OrdenCompraAnularRequest request,
        CancellationToken cancellationToken)
    {
        if (idOrdenCompra <= 0 ||
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

        var orden =
            await _repository.ObtenerPorIdAsync(
                idOrdenCompra,
                idEmpresa,
                cancellationToken);

        if (orden is null)
        {
            return false;
        }

        if (orden.CantidadRecibidaTotal() > 0)
        {
            throw new InvalidOperationException(
                "La orden contiene productos recibidos y no puede anularse.");
        }

        return await _repository.AnularAsync(
            idOrdenCompra,
            idEmpresa,
            idEmpleado,
            request.Motivo,
            cancellationToken);
    }

    private static void Normalizar(
        OrdenCompraDesdeSolicitudRequest request)
    {
        request.Moneda =
            request.Moneda
                .Trim()
                .ToUpperInvariant();

        request.CondicionPago =
            NormalizarTexto(
                request.CondicionPago);

        request.Observacion =
            NormalizarTexto(
                request.Observacion);
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}

internal static class OrdenCompraExtensions
{
    public static decimal CantidadRecibidaTotal(
        this OrdenCompraDetalleResponse orden)
    {
        return orden.Detalles.Sum(
            detalle => detalle.CantidadRecibida);
    }
}