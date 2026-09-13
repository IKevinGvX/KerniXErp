using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.DocumentosProveedor;

public sealed class DocumentoProveedorAnularRequest
{
    [Required]
    [StringLength(1000)]
    public string Observacion { get; set; }
        = string.Empty;
}