namespace KernixERP.DTOs.IA;

public sealed class AnomaliaRegistradaResponse
{
    public long IdAnomalia { get; set; }

    public int IdModeloIA { get; set; }

    public string Modulo { get; set; } = string.Empty;

    public string Entidad { get; set; } = string.Empty;

    public string? IdRegistroOrigen { get; set; }

    public string TipoAnomalia { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string NivelRiesgo { get; set; } = string.Empty;

    public decimal? ValorEsperado { get; set; }

    public decimal? ValorDetectado { get; set; }

    public DateTime FechaDeteccion { get; set; }

    public string Estado { get; set; } = string.Empty;
}