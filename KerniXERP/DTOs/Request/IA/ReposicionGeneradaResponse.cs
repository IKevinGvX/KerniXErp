namespace KernixERP.DTOs.IA;

public sealed class ReposicionGeneradaResponse
{
    public int IdProducto { get; set; }

    public string Producto { get; set; }
        = string.Empty;

    public long IdPrediccionIA { get; set; }

    public long IdRecomendacionIA { get; set; }

    public decimal CantidadSugerida { get; set; }

    public string NivelRiesgo { get; set; }
        = string.Empty;
}