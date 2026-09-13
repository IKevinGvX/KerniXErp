namespace KernixERP.Models;

public sealed class RefreshTokenModel
{
    public long IdRefreshToken { get; set; }

    public int IdUsuario { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public string JwtId { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public DateTime? FechaRevocacion { get; set; }

    public string? ReemplazadoPorHash { get; set; }

    public string? DireccionIp { get; set; }

    public string? Dispositivo { get; set; }
}