using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.SolicitudesCompra.Requests;

public sealed class SolicitudCompraCrearRequest
{
    /*
     * Estos valores serán reemplazados por los datos del JWT
     * desde el controlador.
     */
    [Range(1, int.MaxValue)]
    public int IdEmpresa { get; set; }

    [Range(1, int.MaxValue)]
    public int IdSucursal { get; set; }

    public DateTime? FechaNecesaria { get; set; }

    [StringLength(40)]
    public string? Prioridad { get; set; } = "media";

    [StringLength(1000)]
    public string? Motivo { get; set; }

    [StringLength(1000)]
    public string? Observacion { get; set; }

    [Required]
    [MinLength(1)]
    public List<DetalleSolicitudCompraCrearRequest> Detalles { get; set; } = [];
}