namespace KernixERP.DTOs.OrdenesCompra.Responses;

public sealed class OrdenCompraResumenResponse
{
    public int TotalProductos { get; set; }

    public decimal CantidadTotal { get; set; }

    public decimal CantidadRecibida { get; set; }

    public decimal CantidadPendiente { get; set; }
}