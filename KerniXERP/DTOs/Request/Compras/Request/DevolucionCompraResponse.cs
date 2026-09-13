namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraResponse
{
    public long IdDevolucionCompra { get; set; }

    public string NumeroDevolucion { get; set; } = string.Empty;

    public int IdProveedor { get; set; }

    public string Proveedor { get; set; } = string.Empty;

    public long? IdOrdenCompra { get; set; }

    public string? NumeroOrden { get; set; }

    public long IdRecepcionCompra { get; set; }

    public int IdAlmacen { get; set; }

    public DateTime FechaDevolucion { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public decimal CantidadTotal { get; set; }
}