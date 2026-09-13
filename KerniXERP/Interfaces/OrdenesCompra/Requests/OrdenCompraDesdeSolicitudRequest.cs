using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.OrdenesCompra.Requests;

public sealed class OrdenCompraDesdeSolicitudRequest
{
    [Range(1, long.MaxValue)]
    public long IdSolicitudCompra { get; set; }

    [Range(1, int.MaxValue)]
    public int IdProveedor { get; set; }

    [Required]
    [StringLength(20)]
    public string Moneda { get; set; } = "PEN";

    [Range(
        typeof(decimal),
        "0.000001",
        "999999999999.999999")]
    public decimal TipoCambio { get; set; } = 1;

    public DateTime? FechaEntregaEstimada { get; set; }

    [StringLength(100)]
    public string? CondicionPago { get; set; }

    [StringLength(1000)]
    public string? Observacion { get; set; }

    [Required]
    [MinLength(1)]
    public List<DetalleOrdenDesdeSolicitudRequest> Detalles { get; set; } = [];
}