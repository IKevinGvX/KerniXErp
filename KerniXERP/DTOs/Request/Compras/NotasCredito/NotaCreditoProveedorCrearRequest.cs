using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.NotasCredito;

public sealed class NotaCreditoProveedorCrearRequest
{
    [Range(1, int.MaxValue)]
    public int IdProveedor { get; set; }

    public long? IdDevolucionCompra { get; set; }

    [Range(1, long.MaxValue)]
    public long IdCuentaPorPagar { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroNota { get; set; } = string.Empty;

    [StringLength(100)]
    public string? NumeroDocumentoReferencia { get; set; }

    public DateTime FechaEmision { get; set; }

    [Required]
    [StringLength(20)]
    public string Moneda { get; set; } = "PEN";

    [Range(
        typeof(decimal),
        "0.0001",
        "99999999999999.9999")]
    public decimal Monto { get; set; }

    [Required]
    [StringLength(500)]
    public string Motivo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Observacion { get; set; }
}