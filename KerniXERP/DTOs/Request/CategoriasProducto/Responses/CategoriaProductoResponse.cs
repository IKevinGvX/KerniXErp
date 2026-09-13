namespace KernixERP.DTOs.CategoriasProducto.Responses;

public sealed class CategoriaProductoResponse
{
    public int IdCategoriaProducto { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int TotalProductos { get; set; }
}