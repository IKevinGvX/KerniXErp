using KernixERP.DTOs.Common;
using KernixERP.DTOs.CuentasPorPagar.Requests;
using KernixERP.DTOs.CuentasPorPagar.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class CuentaPorPagarService
    : ICuentaPorPagarService
{
    private readonly ICuentaPorPagarRepository _repository;

    public CuentaPorPagarService(
        ICuentaPorPagarRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<CuentaPorPagarResponse>>
        ListarAsync(
            CuentaPorPagarFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        if (filtro.IdEmpresa is null ||
            filtro.IdEmpresa <= 0)
            throw new ArgumentException(
                "La empresa es obligatoria.");

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

        filtro.Moneda =
            Normalizar(filtro.Moneda)?
                .ToUpperInvariant();

        filtro.Buscar =
            Normalizar(filtro.Buscar);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<CuentaPorPagarDetalleResponse?> ObtenerAsync(
        long idCuentaPorPagar,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idCuentaPorPagar <= 0 ||
            idEmpresa <= 0)
            return Task.FromResult<
                CuentaPorPagarDetalleResponse?>(null);

        return _repository.ObtenerAsync(
            idCuentaPorPagar,
            idEmpresa,
            cancellationToken);
    }

    public Task<long> GenerarDesdeOrdenAsync(
        CuentaPorPagarDesdeOrdenRequest request,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        request.NumeroDocumento =
            Normalizar(request.NumeroDocumento)
            ?? throw new ArgumentException(
                "El número de documento es obligatorio.");

        request.TipoDocumento =
            Normalizar(request.TipoDocumento)?
                .ToLowerInvariant()
            ?? throw new ArgumentException(
                "El tipo de documento es obligatorio.");

        if (request.IdOrdenCompra <= 0)
            throw new ArgumentException(
                "La orden de compra es obligatoria.");

        if (request.FechaEmision == default)
            throw new ArgumentException(
                "La fecha de emisión es obligatoria.");

        if (request.FechaVencimiento.Date <
            request.FechaEmision.Date)
            throw new ArgumentException(
                "La fecha de vencimiento no puede ser anterior a la fecha de emisión.");

        return _repository.GenerarDesdeOrdenAsync(
            request,
            idEmpresa,
            cancellationToken);
    }

    public async Task<bool> RegistrarPagoAsync(
        long idCuentaPorPagar,
        int idEmpresa,
        int idEmpleado,
        PagoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        var cuenta =
            await _repository.ObtenerAsync(
                idCuentaPorPagar,
                idEmpresa,
                cancellationToken);

        if (cuenta is null)
            return false;

        if (cuenta.SaldoPendiente <= 0)
            throw new InvalidOperationException(
                "La cuenta por pagar no tiene saldo pendiente.");

        if (request.Monto <= 0 ||
            request.Monto >
            cuenta.SaldoPendiente)
            throw new ArgumentException(
                "El monto no puede superar el saldo pendiente.");

        request.MetodoPago =
            Normalizar(request.MetodoPago)?
                .ToLowerInvariant()
            ?? throw new ArgumentException(
                "El método de pago es obligatorio.");

        request.Moneda =
            Normalizar(request.Moneda)?
                .ToUpperInvariant()
            ?? cuenta.Moneda;

        request.NumeroOperacion =
            Normalizar(
                request.NumeroOperacion);

        request.Observacion =
            Normalizar(
                request.Observacion);

        await _repository.RegistrarPagoAsync(
            idCuentaPorPagar,
            idEmpleado,
            request,
            cancellationToken);

        return true;
    }

    public Task RevertirPagoAsync(
        long idPagoProveedor,
        PagoProveedorRevertirRequest request,
        CancellationToken cancellationToken)
    {
        if (idPagoProveedor <= 0)
            throw new ArgumentException(
                "El pago no es válido.");

        request.Observacion =
            Normalizar(request.Observacion)
            ?? throw new ArgumentException(
                "La observación de reversión es obligatoria.");

        return _repository.RevertirPagoAsync(
            idPagoProveedor,
            request.Observacion,
            cancellationToken);
    }

    private static string? Normalizar(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}