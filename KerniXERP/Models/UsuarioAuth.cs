namespace KernixERP.Models;

public sealed class UsuarioAuth
{
    public int IdUsuario { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string? Correo { get; set; }

    public string ClaveHash { get; set; } = string.Empty;

    public string? NombreCompleto { get; set; }

    public string? Empresa { get; set; }

    public string? Sucursal { get; set; }

    public bool RequiereCambioClave { get; set; }

    public bool Estado { get; set; }

    public int IntentosFallidos { get; set; }

    public DateTime? BloqueadoHasta { get; set; }
}