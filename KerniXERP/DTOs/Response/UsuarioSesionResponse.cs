namespace KernixERP.DTOs.Responses;

public sealed class UsuarioSesionResponse
{
    public int IdUsuario { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string? Correo { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Empresa { get; set; }

    public string? Sucursal { get; set; }

    public bool RequiereCambioClave { get; set; }
}