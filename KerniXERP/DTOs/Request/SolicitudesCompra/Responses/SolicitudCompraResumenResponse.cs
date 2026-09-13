namespace KernixERP.DTOs.SolicitudesCompra.Responses;

public sealed class SolicitudCompraResumenResponse
{
    public int TotalProductos { get; set; }

    public decimal TotalCantidadSolicitada { get; set; }

    public decimal TotalCantidadAprobada { get; set; }
}