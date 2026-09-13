using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.CuentasPorPagar.Requests;

public sealed class CuentaPorPagarDesdeOrdenRequest
{
    [Range(1, long.MaxValue)]
    public long IdOrdenCompra { get; set; }

    [Required]
    [StringLength(50)]
    public string NumeroDocumento { get; set; } =
        string.Empty;

    [Required]
    [StringLength(20)]
    public string TipoDocumento { get; set; } =
        "factura";

    public DateTime FechaEmision { get; set; }

    public DateTime FechaVencimiento { get; set; }
}