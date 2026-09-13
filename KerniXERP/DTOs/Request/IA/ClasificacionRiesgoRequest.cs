// DTOs/IA/ClasificacionRiesgoRequest.cs

namespace KernixERP.DTOs.IA;

public sealed class ClasificacionRiesgoRequest
{
    public decimal StockDisponible { get; set; }

    public decimal StockMinimo { get; set; }

    public decimal StockProyectado { get; set; }

    public string? NivelRotacion { get; set; }

    public bool EsAnomalo { get; set; }

    public decimal? CantidadMovimiento { get; set; }

    public decimal? UmbralAnomalia { get; set; }
}