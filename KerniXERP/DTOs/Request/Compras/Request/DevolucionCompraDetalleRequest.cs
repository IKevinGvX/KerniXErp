using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraDetalleRequest
{
    [Range(1, long.MaxValue)]
    public long IdDetalleRecepcion { get; set; }

    [Range(typeof(decimal), "0.0001", "99999999999999.9999")]
    public decimal Cantidad { get; set; }

    [StringLength(300)]
    public string? Motivo { get; set; }

    [StringLength(300)]
    public string? Observacion { get; set; }
}