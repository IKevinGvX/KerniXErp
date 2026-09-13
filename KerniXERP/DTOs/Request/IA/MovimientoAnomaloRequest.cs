namespace KernixERP.DTOs.IA;

public sealed class MovimientoAnomaloRequest
{
    public int? IdAlmacen { get; set; }

    public int? IdProducto { get; set; }

    public int DiasHistorial { get; set; } = 30;

    public bool SoloAnomalias { get; set; } = false;
}