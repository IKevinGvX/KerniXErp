namespace KernixERP.DTOs.IA;

public sealed class AlertaReposicionResponse
{
    public long IdAlertaStock { get; set; }

    public int IdProducto { get; set; }

    public int IdAlmacen { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string TipoAlerta { get; set; } = string.Empty;

    public string NivelRiesgo { get; set; } = string.Empty;

    public decimal CantidadSugerida { get; set; }

    public long IdPrediccionIA { get; set; }

    public long IdRecomendacionIA { get; set; }
}