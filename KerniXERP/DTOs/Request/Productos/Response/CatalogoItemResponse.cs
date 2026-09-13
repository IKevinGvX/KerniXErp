namespace KernixERP.DTOs.Productos.Responses;

public sealed class CatalogoItemResponse
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int? IdSucursal { get; set; }
}