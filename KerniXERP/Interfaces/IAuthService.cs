using KernixERP.DTOs.Requests;
using KernixERP.DTOs.Responses;
using KernixERP.Models;

namespace KernixERP.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        string? direccionIp,
        string? dispositivo,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PermisoUsuario>> ObtenerPermisosAsync(
        int idUsuario,
        CancellationToken cancellationToken);
}