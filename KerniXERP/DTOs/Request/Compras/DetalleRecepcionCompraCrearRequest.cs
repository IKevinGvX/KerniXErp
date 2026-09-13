namespace KernixERP.DTOs.Compras;

public sealed class DetalleRecepcionCompraCrearRequest
{
    public long IdDetalleOrdenCompra { get; set; }

    public int IdProducto { get; set; }

    public decimal CantidadEsperada { get; set; }

    public decimal CantidadRecibida { get; set; }

    public decimal CantidadAceptada { get; set; }

    public decimal CantidadRechazada { get; set; }

    public string? CondicionProducto { get; set; }

    public string? Lote { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    public string? Observacion { get; set; }
}