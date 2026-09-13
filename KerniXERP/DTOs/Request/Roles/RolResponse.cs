namespace KerniXERP.DTOs.Request.Roles;

public sealed class RolResponse
{
    public int IdRol { get; set; }

    public int? IdEmpresa { get; set; }

    public string? Empresa { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EsAdministrador { get; set; }

    public bool Estado { get; set; }

    public int TotalUsuarios { get; set; }

    public int TotalPermisos { get; set; }
}