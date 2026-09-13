namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockProductoDetalleResponse
{
    public StockProductoCabeceraResponse Producto { get; set; } = new();

    public List<StockProductoResponse> StockPorAlmacen { get; set; } = [];

    public StockProductoResumenResponse Resumen { get; set; } = new();
}