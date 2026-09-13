using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.Devoluciones;

namespace KernixERP.Interfaces.Compras;

public interface IDevolucionCompraService
{
    Task<DevolucionCompraCrearResponse> RegistrarAsync(
        int idEmpresa,
        int idEmpleado,
        DevolucionCompraCrearRequest request,
        CancellationToken cancellationToken);

    Task<PagedResponse<DevolucionCompraResponse>> ListarAsync(
        int idEmpresa,
        DevolucionCompraFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<DevolucionCompraDetalleResponse?> ObtenerAsync(
        long idDevolucionCompra,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AnularAsync(
        long idDevolucionCompra,
        int idEmpresa,
        int idEmpleado,
        DevolucionCompraAnularRequest request,
        CancellationToken cancellationToken);
}