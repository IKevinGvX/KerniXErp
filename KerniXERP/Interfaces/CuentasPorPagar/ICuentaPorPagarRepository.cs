using KernixERP.DTOs.Common;
using KernixERP.DTOs.CuentasPorPagar.Requests;
using KernixERP.DTOs.CuentasPorPagar.Responses;

namespace KernixERP.Interfaces.Compras;

public interface ICuentaPorPagarRepository
{
    Task<PagedResponse<CuentaPorPagarResponse>> ListarAsync(
        CuentaPorPagarFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<CuentaPorPagarDetalleResponse?> ObtenerAsync(
        long idCuentaPorPagar,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<long> GenerarDesdeOrdenAsync(
        CuentaPorPagarDesdeOrdenRequest request,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task RegistrarPagoAsync(
        long idCuentaPorPagar,
        int idEmpleado,
        PagoProveedorCrearRequest request,
        CancellationToken cancellationToken);

    Task RevertirPagoAsync(
        long idPagoProveedor,
        string observacion,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PagoProveedorResponse>>
        ListarPagosAsync(
            long idCuentaPorPagar,
            int idEmpresa,
            CancellationToken cancellationToken);
}