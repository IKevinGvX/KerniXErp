using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.NotasCredito;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class NotaCreditoProveedorService
    : INotaCreditoProveedorService
{
    private readonly INotaCreditoProveedorRepository _repository;

    public NotaCreditoProveedorService(
        INotaCreditoProveedorRepository repository)
    {
        _repository = repository;
    }

    public Task<long> RegistrarAsync(
        int idEmpresa,
        NotaCreditoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "Empresa no válida.");

        if (request.IdProveedor <= 0)
            throw new ArgumentException(
                "Proveedor no válido.");

        if (request.IdCuentaPorPagar <= 0)
            throw new ArgumentException(
                "Cuenta por pagar no válida.");

        request.NumeroNota =
            Normalizar(request.NumeroNota)
            ?? throw new ArgumentException(
                "Número de nota obligatorio.");

        request.NumeroDocumentoReferencia =
            Normalizar(
                request.NumeroDocumentoReferencia);

        request.Moneda =
            Normalizar(request.Moneda)?
                .ToUpperInvariant()
            ?? "PEN";

        request.Motivo =
            Normalizar(request.Motivo)
            ?? throw new ArgumentException(
                "El motivo es obligatorio.");

        request.Observacion =
            Normalizar(request.Observacion);

        if (request.Monto <= 0)
            throw new ArgumentException(
                "El monto debe ser mayor que cero.");

        if (request.FechaEmision == default)
            throw new ArgumentException(
                "La fecha de emisión es obligatoria.");

        return _repository.RegistrarAsync(
            idEmpresa,
            request,
            cancellationToken);
    }

    public Task<
        PagedResponse<NotaCreditoProveedorResponse>>
        ListarAsync(
            int idEmpresa,
            NotaCreditoProveedorFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        filtro.Pagina =
            Math.Max(
                filtro.Pagina,
                1);

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

    public Task<
        NotaCreditoProveedorDetalleResponse?>
        ObtenerAsync(
            long idNotaCreditoProveedor,
            int idEmpresa,
            CancellationToken cancellationToken)
    {
        if (idNotaCreditoProveedor <= 0)
            return Task.FromResult<
                NotaCreditoProveedorDetalleResponse?>(
                null);

        return _repository.ObtenerAsync(
            idNotaCreditoProveedor,
            idEmpresa,
            cancellationToken);
    }

    public Task AplicarAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idNotaCreditoProveedor <= 0)
            throw new ArgumentException(
                "Nota de crédito no válida.");

        return _repository.AplicarAsync(
            idNotaCreditoProveedor,
            idEmpresa,
            cancellationToken);
    }

    public Task AnularAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        NotaCreditoProveedorAnularRequest request,
        CancellationToken cancellationToken)
    {
        var observacion =
            Normalizar(request.Observacion)
            ?? throw new ArgumentException(
                "Debe indicar el motivo de anulación.");

        return _repository.AnularAsync(
            idNotaCreditoProveedor,
            idEmpresa,
            observacion,
            cancellationToken);
    }

    private static string? Normalizar(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}