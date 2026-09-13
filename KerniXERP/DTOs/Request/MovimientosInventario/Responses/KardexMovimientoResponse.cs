namespace KernixERP.DTOs.MovimientosInventario.Responses;

public sealed class KardexMovimientoResponse
{
    public long IdMovimientoInventario { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public int IdAlmacen { get; set; }

    public string CodigoAlmacen { get; set; } = string.Empty;

    public string Almacen { get; set; } = string.Empty;

    public string Sucursal { get; set; } = string.Empty;

    public int IdTipoMovimiento { get; set; }

    public string CodigoTipoMovimiento { get; set; } = string.Empty;

    public string TipoMovimiento { get; set; } = string.Empty;

    public string Naturaleza { get; set; } = string.Empty;

    public decimal Entrada { get; set; }

    public decimal Salida { get; set; }

    public decimal StockAnterior { get; set; }

    public decimal StockNuevo { get; set; }

    public string? Referencia { get; set; }

    public string? Motivo { get; set; }

    public string Empleado { get; set; } = string.Empty;
}