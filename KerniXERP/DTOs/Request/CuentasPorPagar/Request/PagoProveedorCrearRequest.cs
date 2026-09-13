using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.CuentasPorPagar.Requests;

public sealed class PagoProveedorCrearRequest
{
    [Required]
    [StringLength(30)]
    public string MetodoPago { get; set; } =
        "transferencia";

    [Required]
    [StringLength(10)]
    public string Moneda { get; set; } =
        "PEN";

    [Range(
        typeof(decimal),
        "0.0001",
        "99999999999999.9999")]
    public decimal Monto { get; set; }

    [Range(
        typeof(decimal),
        "0.000001",
        "999999999999.999999")]
    public decimal? TipoCambio { get; set; } = 1;

    [StringLength(100)]
    public string? NumeroOperacion { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }
}