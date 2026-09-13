namespace KernixERP.DTOs.Productos.Responses;

public sealed class ProductoBusquedaResponse
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public decimal PrecioVenta { get; set; }

    public bool ControlaStock { get; set; }

    public bool Estado { get; set; }

    public decimal StockDisponible { get; set; }
}