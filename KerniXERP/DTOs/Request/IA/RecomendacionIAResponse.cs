namespace KernixERP.DTOs.IA;

public sealed class RecomendacionIAResponse
{
    public long IdRecomendacionIA { get; set; }
    public int IdUsuario { get; set; }
    public int IdModeloIA { get; set; }

    public string Modulo { get; set; } = string.Empty;
    public string TipoRecomendacion { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    public string? Prioridad { get; set; }
    public string? ImpactoEstimado { get; set; }

    public string Estado { get; set; } = string.Empty;
    public DateTime FechaGeneracion { get; set; }
}