namespace KernixERP.DTOs.IA;

public sealed class AlertaInventarioResponse
{
    public long IdAlertaStock { get; set; }

    public int IdAlmacen { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; }
        = string.Empty;

    public string Producto { get; set; }
        = string.Empty;

    public string Almacen { get; set; }
        = string.Empty;

    public string TipoAlerta { get; set; }
        = string.Empty;

    public decimal CantidadActual { get; set; }

    public decimal? NivelMinimo { get; set; }

    public decimal? NivelMaximo { get; set; }

    public string Estado { get; set; }
        = string.Empty;

    public DateTime FechaGeneracion { get; set; }

    public DateTime? FechaAtencion { get; set; }
}