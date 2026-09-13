namespace KernixERP.DTOs.IA;

public sealed class RotacionProductoResponse
{
    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; }
        = string.Empty;

    public string Producto { get; set; }
        = string.Empty;

    public decimal CantidadVendida { get; set; }

    public int NumeroVentas { get; set; }

    public decimal ImporteVendido { get; set; }

    public decimal StockActual { get; set; }

    public decimal StockReservado { get; set; }

    public decimal StockDisponible { get; set; }

    public decimal PromedioVentaDiaria { get; set; }

    public decimal PromedioGeneralProductos { get; set; }

    public string NivelRotacion { get; set; }
        = string.Empty;

    public string Diagnostico { get; set; }
        = string.Empty;

    public int DiasHistorial { get; set; }

    public DateTime FechaInicioAnalisis { get; set; }

    public DateTime FechaFinAnalisis { get; set; }
}