namespace KernixERP.DTOs.Proveedores.Requests;

public sealed class ProveedorFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public string? Busqueda { get; set; }

    public bool? Estado { get; set; }
}