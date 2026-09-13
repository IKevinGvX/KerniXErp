namespace KernixERP.DTOs.Productos.Responses;

public sealed class ProductoStockResponse
{
    public int IdStock { get; set; }

    public int IdAlmacen { get; set; }

    public string CodigoAlmacen { get; set; } = string.Empty;

    public string Almacen { get; set; } = string.Empty;

    public int IdSucursal { get; set; }

    public string Sucursal { get; set; } = string.Empty;

    public decimal CantidadActual { get; set; }

    public decimal CantidadReservada { get; set; }

    public decimal CantidadDisponible { get; set; }

    public decimal StockMinimo { get; set; }

    public decimal? StockMaximo { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public string EstadoStock { get; set; } = string.Empty;
}