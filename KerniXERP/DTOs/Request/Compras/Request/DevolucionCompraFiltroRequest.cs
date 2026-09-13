namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraFiltroRequest
{
    public int? IdProveedor { get; set; }

    public int? IdAlmacen { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}