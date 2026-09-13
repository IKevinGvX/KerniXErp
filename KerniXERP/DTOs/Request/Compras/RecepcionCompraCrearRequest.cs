namespace KernixERP.DTOs.Compras;

public sealed class RecepcionCompraCrearRequest
{
    public long IdOrdenCompra { get; set; }

    public int IdAlmacen { get; set; }

    public string? NumeroGuiaRemision { get; set; }

    public DateTime FechaRecepcion { get; set; }

    public string TipoRecepcion { get; set; } = string.Empty;

    public string? Observacion { get; set; }

    public List<DetalleRecepcionCompraCrearRequest> Detalles { get; set; } = [];
}