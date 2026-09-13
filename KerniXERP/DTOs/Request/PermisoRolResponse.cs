namespace KernixERP.DTOs.Roles.Responses;

public sealed class PermisoRolResponse
{
    public int IdPermiso { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Modulo { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;
}