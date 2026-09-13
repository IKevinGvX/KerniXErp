namespace KernixERP.Models;

public sealed class PermisoUsuario
{
    public int IdPermiso { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Modulo { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;

    public int IdRol { get; set; }

    public string Rol { get; set; } = string.Empty;
}