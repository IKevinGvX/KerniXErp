// DTOs/IA/ResultadoAnalisisRequest.cs

namespace KernixERP.DTOs.IA;

public sealed class ResultadoAnalisisRequest
{
    public int IdModeloIA { get; set; }

    public string Modulo { get; set; }
        = string.Empty;

    public string TipoAnalisis { get; set; }
        = string.Empty;

    public string? Entidad { get; set; }

    public string? IdRegistroOrigen { get; set; }

    public string Resultado { get; set; }
        = string.Empty;

    public string? NivelRiesgo { get; set; }

    public int? PuntajeRiesgo { get; set; }
}