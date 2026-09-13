namespace KernixERP.DTOs.IA;

public sealed class MovimientoAnomaloResponse
{
    public long IdMovimientoInventario { get; set; }

    public int IdAlmacen { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; }
        = string.Empty;

    public string Producto { get; set; }
        = string.Empty;

    public int IdTipoMovimiento { get; set; }

    public int IdEmpleado { get; set; }

    public decimal Cantidad { get; set; }

    public decimal StockAnterior { get; set; }

    public decimal StockNuevo { get; set; }

    public string? Referencia { get; set; }

    public string? Motivo { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public decimal PromedioHistorico { get; set; }

    public decimal DesviacionHistorica { get; set; }

    public decimal UmbralAnomalia { get; set; }

    public string NivelRiesgo { get; set; }
        = string.Empty;

    public bool EsAnomalo { get; set; }

    public int NumeroMovimientos { get; set; }
}