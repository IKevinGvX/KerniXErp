using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.MovimientosInventario.Requests;

public sealed class MovimientoInventarioCrearRequest
{
    [Range(1, int.MaxValue)]
    public int IdAlmacen { get; set; }

    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int IdTipoMovimiento { get; set; }

    [Range(
        typeof(decimal),
        "0.0001",
        "999999999999.9999")]
    public decimal Cantidad { get; set; }

    [StringLength(100)]
    public string? Referencia { get; set; }

    [StringLength(300)]
    public string? Motivo { get; set; }
}