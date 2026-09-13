namespace KernixERP.Models.Compras;

public sealed class RecepcionCompra
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
}