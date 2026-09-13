using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.CuentasPorPagar.Requests;

public sealed class PagoProveedorRevertirRequest
{
    [Required]
    [StringLength(300)]
    public string Observacion { get; set; } =
        string.Empty;
}