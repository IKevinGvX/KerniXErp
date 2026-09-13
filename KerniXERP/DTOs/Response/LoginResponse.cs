using KernixERP.Models;

namespace KernixERP.DTOs.Responses;

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime AccessTokenExpiresAt { get; set; }

    public DateTime RefreshTokenExpiresAt { get; set; }

    public UsuarioSesionResponse Usuario { get; set; } = new();

    public IReadOnlyCollection<string> Roles { get; set; } = [];

    public IReadOnlyCollection<PermisoUsuario> Permisos { get; set; } = [];
}