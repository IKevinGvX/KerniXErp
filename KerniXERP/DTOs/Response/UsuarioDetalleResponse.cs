namespace KernixERP.DTOs.Usuarios.Responses;

public sealed class UsuarioDetalleResponse
{
    public int IdUsuario { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string? Correo { get; set; }

    public string? Empleado { get; set; }

    public string? Empresa { get; set; }

    public string? Sucursal { get; set; }

    public bool Estado { get; set; }

    public bool RequiereCambioClave { get; set; }

    public int IntentosFallidos { get; set; }

    public DateTime? BloqueadoHasta { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public List<string> Roles { get; set; } = [];

    public List<string> Permisos { get; set; } = [];
}