namespace KernixERP.Models.IA;

public sealed class PrediccionIA
{
    public long IdPrediccionIA { get; set; }

    public int IdModeloIA { get; set; }

    public int IdUsuario { get; set; }

    public string TipoPrediccion { get; set; } = string.Empty;

    public DateTime FechaInicioPeriodo { get; set; }

    public DateTime FechaFinPeriodo { get; set; }

    public decimal? ValorPredicho { get; set; }

    public string UnidadMedida { get; set; } = string.Empty;

    public decimal? NivelConfianza { get; set; }

    public string? Parametros { get; set; }

    public DateTime FechaGeneracion { get; set; }
}