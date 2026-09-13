namespace KernixERP.DTOs.Compras.DocumentosProveedor;

public sealed class DocumentoProveedorFiltroRequest
{
    public int? IdProveedor { get; set; }

    public string? CodigoTipo { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}