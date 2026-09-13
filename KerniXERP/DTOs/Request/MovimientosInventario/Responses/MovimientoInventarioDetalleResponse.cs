namespace KernixERP.DTOs.MovimientosInventario.Responses;

public sealed class MovimientoInventarioDetalleResponse
{
    public long IdMovimientoInventario { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public int IdSucursal { get; set; }

    public string Sucursal { get; set; } = string.Empty;

    public int IdAlmacen { get; set; }

    public string CodigoAlmacen { get; set; } = string.Empty;

    public string Almacen { get; set; } = string.Empty;

    public string? UbicacionAlmacen { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string? DescripcionProducto { get; set; }

    public int IdTipoMovimiento { get; set; }

    public string CodigoTipoMovimiento { get; set; } = string.Empty;

    public string TipoMovimiento { get; set; } = string.Empty;

    public string Naturaleza { get; set; } = string.Empty;

    public string? DescripcionTipoMovimiento { get; set; }

    public int IdEmpleado { get; set; }

    public string Empleado { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal StockAnterior { get; set; }

    public decimal StockNuevo { get; set; }

    public string? Referencia { get; set; }

    public string? Motivo { get; set; }

    public DateTime FechaMovimiento { get; set; }
}