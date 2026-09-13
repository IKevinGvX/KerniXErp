using KernixERP.DTOs.Common;
using KernixERP.DTOs.OrdenesCompra.Requests;
using KernixERP.DTOs.OrdenesCompra.Responses;

namespace KernixERP.Interfaces.Compras;

public interface IOrdenCompraService
{
    Task<PagedResponse<OrdenCompraResponse>> ListarAsync(
        OrdenCompraFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<OrdenCompraDetalleResponse?> ObtenerPorIdAsync(
        long idOrdenCompra,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<(long IdOrdenCompra, string NumeroOrden)>
        GenerarDesdeSolicitudAsync(
            OrdenCompraDesdeSolicitudRequest request,
            int idEmpresa,
            int idEmpleado,
            CancellationToken cancellationToken);

    Task<bool> AnularAsync(
        long idOrdenCompra,
        int idEmpresa,
        int idEmpleado,
        OrdenCompraAnularRequest request,
        CancellationToken cancellationToken);
}