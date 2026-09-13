namespace KernixERP.Models.Compras;

public sealed class DetalleRecepcionCompra
{
    public long IdDetalleRecepcion { get; set; }

    public long IdRecepcionCompra { get; set; }

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