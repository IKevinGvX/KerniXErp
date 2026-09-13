using KernixERP.DTOs.Almacenes.Requests;
using KernixERP.DTOs.Almacenes.Responses;
using KernixERP.DTOs.Common;

namespace KernixERP.Interfaces.Inventario;

public interface IAlmacenRepository
{
    Task<PagedResponse<AlmacenResponse>> ListarAsync(
        AlmacenFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<AlmacenDetalleResponse?> ObtenerPorIdAsync(
        int idAlmacen,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        AlmacenCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idAlmacen,
        AlmacenActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idAlmacen,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}