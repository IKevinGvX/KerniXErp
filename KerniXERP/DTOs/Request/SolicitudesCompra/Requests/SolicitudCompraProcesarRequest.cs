using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.SolicitudesCompra.Requests;

public sealed class SolicitudCompraProcesarRequest
{
    [Required]
    [StringLength(1000)]
    public string Observacion { get; set; } = string.Empty;
}