namespace KernixERP.DTOs.IA;

public sealed class AlertaInventarioFiltroRequest
{
    public int? IdAlmacen { get; set; }

    public string? TipoAlerta { get; set; }

    public string? Estado { get; set; }
}