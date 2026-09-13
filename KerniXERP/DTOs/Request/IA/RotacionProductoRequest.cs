namespace KernixERP.DTOs.IA;

public sealed class RotacionProductoRequest
{
    public int? IdAlmacen { get; set; }

    public int DiasHistorial { get; set; } = 30;
}