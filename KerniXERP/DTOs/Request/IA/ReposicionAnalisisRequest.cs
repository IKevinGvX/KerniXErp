namespace KernixERP.DTOs.IA;

public sealed class ReposicionAnalisisRequest
{
    public int? IdAlmacen { get; set; }

    public int DiasHistorial { get; set; } = 30;

    public int DiasProyeccion { get; set; } = 30;
}