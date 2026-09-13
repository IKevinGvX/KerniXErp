namespace KernixERP.DTOs.IA;

public sealed class SolicitudCompraDesdeIAResponse
{
    public long IdRecomendacionIA { get; set; }

    public long IdSolicitudCompra { get; set; }

    public string NumeroSolicitud { get; set; } = string.Empty;

    public int IdProducto { get; set; }

    public decimal CantidadSolicitada { get; set; }
}