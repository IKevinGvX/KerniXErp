using KernixERP.DTOs.Common;
using KernixERP.DTOs.Stock.Requests;
using KernixERP.DTOs.Stock.Responses;

namespace KernixERP.Interfaces.Inventario;

public interface IStockRepository
{
    Task<PagedResponse<StockResponse>> ListarAsync(
        StockFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<StockDetalleResponse?> ObtenerPorIdAsync(
        int idStock,
        CancellationToken cancellationToken);

    Task<StockProductoDetalleResponse?> ObtenerPorProductoAsync(
        int idProducto,
        int? idEmpresa,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idStock,
        StockActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<StockDashboardResponse> ObtenerDashboardAsync(
        int idEmpresa,
        int? idSucursal,
        int? idAlmacen,
        CancellationToken cancellationToken);
}