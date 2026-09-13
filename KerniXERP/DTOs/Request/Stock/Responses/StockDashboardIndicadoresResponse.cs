namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockDashboardIndicadoresResponse
{
    public int TotalProductos { get; set; }

    public int TotalAlmacenes { get; set; }

    public int RegistrosSinStock { get; set; }

    public int RegistrosStockBajo { get; set; }

    public int RegistrosSobreStock { get; set; }

    public decimal CantidadActual { get; set; }

    public decimal CantidadReservada { get; set; }

    public decimal CantidadDisponible { get; set; }

    public decimal ValorInventarioCompra { get; set; }

    public decimal ValorInventarioVenta { get; set; }
}