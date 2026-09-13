namespace KernixERP.DTOs.Compras.NotasCredito;

public sealed class NotaCreditoProveedorFiltroRequest
{
    public int? IdProveedor { get; set; }

    public long? IdCuentaPorPagar { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}