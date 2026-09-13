namespace KernixERP.DTOs.OrdenesCompra.Responses;

public sealed class DetalleOrdenCompraResponse
{
    public long IdDetalleOrdenCompra { get; set; }

    public long IdOrdenCompra { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public string AbreviaturaUnidad { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Total { get; set; }

    public decimal CantidadRecibida { get; set; }

    public decimal CantidadPendiente { get; set; }
}