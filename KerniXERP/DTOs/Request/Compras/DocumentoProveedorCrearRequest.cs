using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.DocumentosProveedor;

public sealed class DocumentoProveedorCrearRequest
{
    [Required]
    [StringLength(10)]
    public string CodigoTipoDocumento { get; set; }
        = string.Empty;

    [Range(1, int.MaxValue)]
    public int IdProveedor { get; set; }

    public long? IdCuentaPorPagar { get; set; }

    public long? IdDevolucionCompra { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroDocumento { get; set; }
        = string.Empty;

    [StringLength(100)]
    public string? NumeroDocumentoReferencia { get; set; }

    public DateTime FechaEmision { get; set; }

    [Required]
    [StringLength(20)]
    public string Moneda { get; set; }
        = "PEN";

    [Range(
        typeof(decimal),
        "0.0001",
        "99999999999999.9999")]
    public decimal Total { get; set; }

    [StringLength(500)]
    public string? Motivo { get; set; }

    [StringLength(1000)]
    public string? Observacion { get; set; }
}