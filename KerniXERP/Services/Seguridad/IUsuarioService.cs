using KernixERP.DTOs.Common;
using KernixERP.DTOs.Usuarios.Requests;
using KernixERP.DTOs.Usuarios.Responses;

namespace KernixERP.Interfaces.Seguridad;

public interface IUsuarioService
{
    Task<PagedResponse<UsuarioResponse>> ListarAsync(
        UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        UsuarioCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idUsuario,
        UsuarioActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idUsuario,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarPasswordAsync(
        int idUsuario,
        UsuarioCambiarPasswordRequest request,
        CancellationToken cancellationToken);

    Task<bool> AsignarRolesAsync(
        int idUsuario,
        UsuarioRolesRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);
}