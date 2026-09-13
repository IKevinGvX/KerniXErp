namespace KernixERP.DTOs.Compras;

public sealed class RecepcionCompraDetalleResponse
{
    public long IdRecepcionCompra { get; set; }

    public long IdOrdenCompra { get; set; }

    public int IdAlmacen { get; set; }

    public int IdEmpleado { get; set; }

    public string NumeroRecepcion { get; set; } = string.Empty;

    public string? NumeroGuiaRemision { get; set; }

    public DateTime FechaRecepcion { get; set; }

    public string TipoRecepcion { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public string? Observacion { get; set; }

    public List<DetalleRecepcionCompraResponse> Detalles { get; set; } = [];
}

public sealed class DetalleRecepcionCompraResponse
{
    public long IdDetalleRecepcion { get; set; }

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