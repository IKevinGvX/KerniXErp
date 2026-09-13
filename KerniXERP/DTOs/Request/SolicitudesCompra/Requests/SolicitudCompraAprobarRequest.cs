using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.SolicitudesCompra.Requests;

public sealed class SolicitudCompraAprobarRequest
{
    [StringLength(1000)]
    public string? Observacion { get; set; }

    [Required]
    [MinLength(1)]
    public List<DetalleSolicitudCompraAprobarRequest> Detalles { get; set; } = [];
}