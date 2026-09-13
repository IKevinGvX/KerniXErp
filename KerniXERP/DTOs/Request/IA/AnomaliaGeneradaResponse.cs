namespace KernixERP.DTOs.IA;

public sealed class AnomaliaGeneradaResponse
{
    public long IdAnomalia { get; set; }

    public long IdMovimientoInventario { get; set; }

    public int IdProducto { get; set; }

    public string Producto { get; set; }
        = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal UmbralAnomalia { get; set; }

    public string NivelRiesgo { get; set; }
        = string.Empty;

    public string Estado { get; set; }
        = "detectada";
}