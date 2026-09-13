namespace KernixERP.DTOs.IA;

public sealed class RecomendacionReposicionResponse
{
    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string Producto { get; set; } = string.Empty;

    public int IdAlmacen { get; set; }

    public string Almacen { get; set; } = string.Empty;

    public decimal StockActual { get; set; }

    public decimal StockReservado { get; set; }

    public decimal StockDisponible { get; set; }

    public decimal StockMinimo { get; set; }

    public decimal? StockMaximo { get; set; }

    public decimal ConsumoPeriodo { get; set; }

    public decimal ConsumoPromedioDiario { get; set; }

    public decimal DemandaProyectada { get; set; }

    public decimal StockProyectado { get; set; }

    public decimal CantidadSugerida { get; set; }

    public string NivelRiesgo { get; set; } = string.Empty;

    public string Recomendacion { get; set; } = string.Empty;

    public int DiasHistorial { get; set; }

    public int DiasProyeccion { get; set; }
}