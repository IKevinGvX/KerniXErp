using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraAnularRequest
{
    [Required]
    [StringLength(500)]
    public string Observacion { get; set; } = string.Empty;
}