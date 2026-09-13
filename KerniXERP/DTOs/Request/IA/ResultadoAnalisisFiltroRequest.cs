// DTOs/IA/ResultadoAnalisisFiltroRequest.cs

namespace KernixERP.DTOs.IA;

public sealed class ResultadoAnalisisFiltroRequest
{
    public string? Modulo { get; set; }

    public string? TipoAnalisis { get; set; }

    public string? NivelRiesgo { get; set; }
}