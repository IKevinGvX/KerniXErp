namespace KernixERP.DTOs.IA;

public sealed class PronosticoVentaRequest
{
    public int? IdProducto { get; set; }

    public int DiasHistorial { get; set; } = 30;

    public int DiasProyeccion { get; set; } = 30;
}