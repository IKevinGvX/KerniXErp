using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.OrdenesCompra.Requests;

public sealed class OrdenCompraAnularRequest
{
    [Required]
    [StringLength(1000)]
    public string Motivo { get; set; } = string.Empty;
}