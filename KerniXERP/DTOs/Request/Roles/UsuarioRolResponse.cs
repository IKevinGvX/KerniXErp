namespace KernixERP.DTOs.Roles.Responses;

public sealed class UsuarioRolResponse
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string? Correo { get; set; }
}