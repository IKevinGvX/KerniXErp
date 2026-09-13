using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.SolicitudesCompra.Requests;

public sealed class DetalleSolicitudCompraAprobarRequest
{
    [Range(1, long.MaxValue)]
    public long IdDetalleSolicitud { get; set; }

    [Range(
        typeof(decimal),
        "0.0001",
        "99999999999999.9999")]
    public decimal CantidadAprobada { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }
}