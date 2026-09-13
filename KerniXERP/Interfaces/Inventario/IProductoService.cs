using KernixERP.DTOs.Common;
using KernixERP.DTOs.Productos.Requests;
using KernixERP.DTOs.Productos.Responses;

namespace KernixERP.Interfaces.Inventario;

public interface IProductoService
{
    Task<PagedResponse<ProductoResponse>> ListarAsync(
        ProductoFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<ProductoDetalleResponse?> ObtenerPorIdAsync(
        int idProducto,
        CancellationToken cancellationToken);

    Task<ProductoCatalogosResponse> ObtenerCatalogosAsync(
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<ProductoBusquedaResponse?> BuscarCodigoAsync(
        int idEmpresa,
        string codigo,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        ProductoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idProducto,
        ProductoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idProducto,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}