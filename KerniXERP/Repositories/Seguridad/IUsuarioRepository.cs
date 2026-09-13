using KernixERP.DTOs.Common;
using KernixERP.DTOs.Usuarios.Requests;
using KernixERP.DTOs.Usuarios.Responses;

namespace KernixERP.Interfaces.Seguridad;

public interface IUsuarioRepository
{
    Task<PagedResponse<UsuarioResponse>> ListarAsync(
        UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        UsuarioCrearRequest request,
        string claveHash,
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

    Task<bool> ActualizarClaveAsync(
        int idUsuario,
        string claveHash,
        bool requiereCambioClave,
        CancellationToken cancellationToken);

    Task<bool> AsignarRolesAsync(
        int idUsuario,
        IReadOnlyCollection<int> idRoles,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<string?> ObtenerHashAsync(
        int idUsuario,
        CancellationToken cancellationToken);
}