namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockDashboardResponse
{
    public StockDashboardIndicadoresResponse Indicadores { get; set; } = new();

    public List<StockCriticoResponse> StockCritico { get; set; } = [];

    public List<StockAlmacenResumenResponse> Almacenes { get; set; } = [];
}