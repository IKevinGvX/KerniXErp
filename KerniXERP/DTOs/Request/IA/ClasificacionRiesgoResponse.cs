// DTOs/IA/ClasificacionRiesgoResponse.cs

namespace KernixERP.DTOs.IA;

public sealed class ClasificacionRiesgoResponse
{
    public int PuntajeRiesgo { get; set; }

    public string NivelRiesgo { get; set; }
        = string.Empty;

    public string Motivo { get; set; }
        = string.Empty;

    public decimal StockDisponible { get; set; }

    public decimal StockMinimo { get; set; }

    public decimal StockProyectado { get; set; }

    public string NivelRotacion { get; set; }
        = string.Empty;

    public bool EsAnomalo { get; set; }
}