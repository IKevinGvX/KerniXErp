namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockCriticoResponse
{
    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string Producto { get; set; } = string.Empty;

    public int IdAlmacen { get; set; }

    public string Almacen { get; set; } = string.Empty;

    public string Sucursal { get; set; } = string.Empty;

    public decimal CantidadActual { get; set; }

    public decimal CantidadReservada { get; set; }

    public decimal CantidadDisponible { get; set; }

    public decimal StockMinimo { get; set; }

    public string EstadoStock { get; set; } = string.Empty;
}