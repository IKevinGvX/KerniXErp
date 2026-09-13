namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockAlmacenResumenResponse
{
    public int IdAlmacen { get; set; }

    public string Almacen { get; set; } = string.Empty;

    public int IdSucursal { get; set; }

    public string Sucursal { get; set; } = string.Empty;

    public int TotalProductos { get; set; }

    public decimal CantidadActual { get; set; }

    public decimal CantidadReservada { get; set; }

    public decimal CantidadDisponible { get; set; }

    public decimal ValorInventarioCompra { get; set; }
}