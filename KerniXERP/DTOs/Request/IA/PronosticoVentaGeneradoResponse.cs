namespace KernixERP.DTOs.IA;

public sealed class PronosticoVentaGeneradoResponse
{
    public long IdPrediccionIA { get; set; }

    public int IdProducto { get; set; }

    public string Producto { get; set; }
        = string.Empty;
    public string Tendencia { get; set; }
    = string.Empty;
    public decimal CantidadPronosticada { get; set; }

    public decimal NivelConfianza { get; set; }

    public int DiasProyeccion { get; set; }
}