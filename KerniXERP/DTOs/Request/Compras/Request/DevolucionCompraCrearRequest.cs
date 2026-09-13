using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraCrearRequest
{
    [Range(1, long.MaxValue)]
    public long IdRecepcionCompra { get; set; }

    [Range(1, int.MaxValue)]
    public int IdAlmacen { get; set; }

    [Required]
    [StringLength(300)]
    public string Motivo { get; set; } = string.Empty;

    [StringLength(50)]
    public string? NumeroDocumentoReferencia { get; set; }

    [StringLength(500)]
    public string? Observacion { get; set; }

    [Required]
    [MinLength(1)]
    public List<DevolucionCompraDetalleRequest> Detalles { get; set; } = [];
}