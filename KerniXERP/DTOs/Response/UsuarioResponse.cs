namespace KernixERP.DTOs.Usuarios.Responses;

public sealed class UsuarioResponse
{
    public int IdUsuario { get; set; }

    public int? IdEmpleado { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string? Correo { get; set; }

    public string? Empleado { get; set; }

    public string? Empresa { get; set; }

    public string? Sucursal { get; set; }

    public bool Estado { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public string Roles { get; set; } = string.Empty;
}   