using KernixERP.DTOs.CategoriasProducto.Requests;
using KernixERP.DTOs.CategoriasProducto.Responses;
using KernixERP.DTOs.Common;

namespace KernixERP.Interfaces.Inventario;

public interface ICategoriaProductoService
{
    Task<PagedResponse<CategoriaProductoResponse>> ListarAsync(
        CategoriaProductoFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<CategoriaProductoDetalleResponse?> ObtenerPorIdAsync(
        int idCategoriaProducto,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        CategoriaProductoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idCategoriaProducto,
        CategoriaProductoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idCategoriaProducto,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}