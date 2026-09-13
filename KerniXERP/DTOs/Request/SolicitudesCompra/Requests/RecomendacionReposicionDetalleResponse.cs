namespace KernixERP.DTOs.IA;

public sealed class RecomendacionReposicionDetalleResponse
{
    public long IdRecomendacionIA { get; set; }

    public int IdProducto { get; set; }

    public int IdAlmacen { get; set; }

    public decimal CantidadSugerida { get; set; }

    public string? Prioridad { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
}