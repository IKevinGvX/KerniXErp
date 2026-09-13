using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.OrdenesCompra.Requests;

public sealed class DetalleOrdenDesdeSolicitudRequest
{
    [Range(1, long.MaxValue)]
    public long IdDetalleSolicitud { get; set; }

    [Range(
        typeof(decimal),
        "0.0001",
        "99999999999999.9999")]
    public decimal PrecioUnitario { get; set; }

    [Range(
        typeof(decimal),
        "0",
        "99999999999999.9999")]
    public decimal Descuento { get; set; }

    [Range(
        typeof(decimal),
        "0",
        "100")]
    public decimal PorcentajeIgv { get; set; } = 18;
}