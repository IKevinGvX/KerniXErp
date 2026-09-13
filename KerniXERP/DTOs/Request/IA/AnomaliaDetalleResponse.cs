namespace KernixERP.DTOs.IA;

public sealed class AnomaliaDetalleResponse
{
    public long IdAnomalia { get; set; }

    public int IdModeloIA { get; set; }

    public string Modulo { get; set; } = string.Empty;

    public string Entidad { get; set; } = string.Empty;

    public string? IdRegistroOrigen { get; set; }

    public string TipoAnomalia { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string NivelRiesgo { get; set; } = string.Empty;

    public decimal? ValorEsperado { get; set; }

    public decimal? ValorDetectado { get; set; }

    public DateTime FechaDeteccion { get; set; }

    public string Estado { get; set; } = string.Empty;

    // Movimiento de inventario
    public long IdMovimientoInventario { get; set; }

    public int IdAlmacen { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string Producto { get; set; } = string.Empty;

    public decimal CantidadMovimiento { get; set; }

    public decimal StockAnterior { get; set; }

    public decimal StockNuevo { get; set; }

    public string? Referencia { get; set; }

    public string? Motivo { get; set; }

    public DateTime FechaMovimiento { get; set; }
}