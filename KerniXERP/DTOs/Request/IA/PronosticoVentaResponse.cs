namespace KernixERP.DTOs.IA;

public sealed class PronosticoVentaResponse
{
    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; }
        = string.Empty;

    public string Producto { get; set; }
        = string.Empty;

    public decimal CantidadVendidaHistorica { get; set; }

    public decimal PromedioVentaDiaria { get; set; }

    public decimal CantidadPronosticada { get; set; }

    public decimal ImporteHistorico { get; set; }

    public int NumeroVentas { get; set; }

    public int DiasHistorial { get; set; }

    public int DiasProyeccion { get; set; }

    public DateTime FechaInicioHistorial { get; set; }
    public decimal VentasPeriodoAnterior { get; set; }

    public decimal VentasPeriodoReciente { get; set; }

    public string Tendencia { get; set; }
        = string.Empty;
    public DateTime FechaFinHistorial { get; set; }
}