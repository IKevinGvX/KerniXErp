using KernixERP.Models;

namespace KernixERP.Interfaces;

public interface IAuthRepository
{
    Task<UsuarioAuth?> ObtenerUsuarioAsync(
        string usuario,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> ObtenerRolesAsync(
        int idUsuario,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PermisoUsuario>> ObtenerPermisosAsync(
        int idUsuario,
        CancellationToken cancellationToken);

    Task RegistrarAccesoAsync(
        int? idUsuario,
        string usuarioIngresado,
        string? direccionIp,
        string? dispositivo,
        string resultado,
        string? motivoFallo,
        CancellationToken cancellationToken);

    Task RegistrarLoginExitosoAsync(
        int idUsuario,
        CancellationToken cancellationToken);

    Task RegistrarLoginFallidoAsync(
        int idUsuario,
        CancellationToken cancellationToken);
    Task GuardarRefreshTokenAsync(
    RefreshTokenModel refreshToken,
    CancellationToken cancellationToken);

    Task<RefreshTokenModel?> ObtenerRefreshTokenAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task RevocarRefreshTokenAsync(
        long idRefreshToken,
        string motivo,
        string? reemplazadoPorHash,
        CancellationToken cancellationToken);

    Task RevocarTokensUsuarioAsync(
        int idUsuario,
        string motivo,
        CancellationToken cancellationToken);

    Task<UsuarioAuth?> ObtenerUsuarioPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken);
}