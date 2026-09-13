using KernixERP.DTOs.Common;
using KernixERP.DTOs.Sucursales.Requests;
using KernixERP.DTOs.Sucursales.Responses;

namespace KernixERP.Interfaces.Organizacion;

public interface ISucursalService
{
    Task<PagedResponse<SucursalResponse>> ListarAsync(
        SucursalFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<SucursalDetalleResponse?> ObtenerPorIdAsync(
        int idSucursal,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        SucursalCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idSucursal,
        SucursalActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idSucursal,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}