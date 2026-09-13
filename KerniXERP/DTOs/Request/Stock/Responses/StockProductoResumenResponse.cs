namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockProductoResumenResponse
{
    public int TotalAlmacenes { get; set; }

    public decimal CantidadActual { get; set; }

    public decimal CantidadReservada { get; set; }

    public decimal CantidadDisponible { get; set; }

    public decimal ValorCompra { get; set; }

    public decimal ValorVenta { get; set; }
}