namespace KernixERP.DTOs.CuentasPorPagar.Requests;

public sealed class CuentaPorPagarFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdProveedor { get; set; }

    public string? Estado { get; set; }

    public string? Moneda { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}