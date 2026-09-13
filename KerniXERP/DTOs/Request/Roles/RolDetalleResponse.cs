namespace KernixERP.DTOs.Roles.Responses;

public sealed class RolDetalleResponse
{
    public int IdRol { get; set; }

    public int? IdEmpresa { get; set; }

    public string? Empresa { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool EsAdministrador { get; set; }

    public bool Estado { get; set; }

    public List<PermisoRolResponse> Permisos { get; set; } = [];

    public List<UsuarioRolResponse> Usuarios { get; set; } = [];
}