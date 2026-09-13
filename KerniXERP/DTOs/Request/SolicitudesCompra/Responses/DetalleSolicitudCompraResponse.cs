namespace KernixERP.DTOs.SolicitudesCompra.Responses;

public sealed class DetalleSolicitudCompraResponse
{
    public long IdDetalleSolicitud { get; set; }

    public long IdSolicitudCompra { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string? Marca { get; set; }

    public string UnidadMedida { get; set; } = string.Empty;

    public string AbreviaturaUnidad { get; set; } = string.Empty;

    public decimal CantidadSolicitada { get; set; }

    public decimal? CantidadAprobada { get; set; }

    public string? Motivo { get; set; }

    public string? Observacion { get; set; }
}