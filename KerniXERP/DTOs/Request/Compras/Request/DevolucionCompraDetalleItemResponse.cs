namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraDetalleItemResponse
{
    public long IdDetalleDevolucion { get; set; }

    public long IdDetalleRecepcion { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string Producto { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public string? Motivo { get; set; }

    public string? Lote { get; set; }

    public string? Observacion { get; set; }
}