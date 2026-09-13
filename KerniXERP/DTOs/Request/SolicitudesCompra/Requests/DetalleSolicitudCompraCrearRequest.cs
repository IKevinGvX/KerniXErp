using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.SolicitudesCompra.Requests;

public sealed class DetalleSolicitudCompraCrearRequest
{
    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(
        typeof(decimal),
        "0.0001",
        "99999999999999.9999")]
    public decimal CantidadSolicitada { get; set; }

    [StringLength(500)]
    public string? Motivo { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }
}