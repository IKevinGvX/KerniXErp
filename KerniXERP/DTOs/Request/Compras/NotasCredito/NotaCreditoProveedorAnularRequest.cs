using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.NotasCredito;

public sealed class NotaCreditoProveedorAnularRequest
{
    [Required]
    [StringLength(1000)]
    public string Observacion { get; set; } = string.Empty;
}