using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.Devoluciones;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class DevolucionCompraService
    : IDevolucionCompraService
{
    private readonly IDevolucionCompraRepository _repository;

    public DevolucionCompraService(
        IDevolucionCompraRepository repository)
    {
        _repository = repository;
    }

    public Task<DevolucionCompraCrearResponse> RegistrarAsync(
        int idEmpresa,
        int idEmpleado,
        DevolucionCompraCrearRequest request,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "Empresa no válida.");

        if (idEmpleado <= 0)
            throw new ArgumentException(
                "Empleado no válido.");

        if (request.IdRecepcionCompra <= 0)
            throw new ArgumentException(
                "La recepción de compra es obligatoria.");

        if (request.IdAlmacen <= 0)
            throw new ArgumentException(
                "El almacén es obligatorio.");

        request.Motivo =
            Normalizar(request.Motivo)
            ?? throw new ArgumentException(
                "El motivo es obligatorio.");

        request.NumeroDocumentoReferencia =
            Normalizar(
                request.NumeroDocumentoReferencia);

        request.Observacion =
            Normalizar(request.Observacion);

        if (request.Detalles is null ||
            request.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "Debe registrar al menos un producto.");
        }

        if (request.Detalles.Any(
                x => x.IdDetalleRecepcion <= 0))
        {
            throw new ArgumentException(
                "Existe un detalle de recepción no válido.");
        }

        if (request.Detalles.Any(
                x => x.Cantidad <= 0))
        {
            throw new ArgumentException(
                "Todas las cantidades deben ser mayores que cero.");
        }

        if (request.Detalles
            .GroupBy(x => x.IdDetalleRecepcion)
            .Any(g => g.Count() > 1))
        {
            throw new ArgumentException(
                "No se puede repetir el mismo detalle de recepción.");
        }

        foreach (var detalle in request.Detalles)
        {
            detalle.Motivo =
                Normalizar(detalle.Motivo);

            detalle.Observacion =
                Normalizar(detalle.Observacion);
        }

        return _repository.RegistrarAsync(
            idEmpresa,
            idEmpleado,
            request,
            cancellationToken);
    }

    public Task<PagedResponse<DevolucionCompraResponse>> ListarAsync(
        int idEmpresa,
        DevolucionCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "Empresa no válida.");

        filtro.Pagina =
            Math.Max(filtro.Pagina, 1);

        filtro.TamanoPagina =
            Math.Clamp(
                filtro.TamanoPagina,
                1,
                100);

        filtro.Estado =
            Normalizar(filtro.Estado)?
                .ToLowerInvariant();

        filtro.Buscar =
            Normalizar(filtro.Buscar);

        return _repository.ListarAsync(
            idEmpresa,
            filtro,
            cancellationToken);
    }

    public Task<DevolucionCompraDetalleResponse?> ObtenerAsync(
        long idDevolucionCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idDevolucionCompra <= 0)
            return Task.FromResult<
                DevolucionCompraDetalleResponse?>(null);

        return _repository.ObtenerAsync(
            idDevolucionCompra,
            idEmpresa,
            cancellationToken);
    }

    public Task AnularAsync(
        long idDevolucionCompra,
        int idEmpresa,
        int idEmpleado,
        DevolucionCompraAnularRequest request,
        CancellationToken cancellationToken)
    {
        if (idDevolucionCompra <= 0)
            throw new ArgumentException(
                "Devolución no válida.");

        var observacion =
            Normalizar(request.Observacion)
            ?? throw new ArgumentException(
                "Debe indicar el motivo de la anulación.");

        return _repository.AnularAsync(
            idDevolucionCompra,
            idEmpresa,
            idEmpleado,
            observacion,
            cancellationToken);
    }

    private static string? Normalizar(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}