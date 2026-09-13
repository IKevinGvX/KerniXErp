namespace KernixERP.DTOs.Productos.Requests;

public sealed class ProductoFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdCategoriaProducto { get; set; }

    public int? IdMarca { get; set; }

    public int? IdUnidadMedida { get; set; }

    public int? IdAlmacen { get; set; }

    public bool? Estado { get; set; }

    public bool? ControlaStock { get; set; }

    public string? EstadoStock { get; set; }

    public decimal? PrecioDesde { get; set; }

    public decimal? PrecioHasta { get; set; }

    public string? Buscar { get; set; }

    public string OrdenarPor { get; set; } = "nombre";

    public string DireccionOrden { get; set; } = "asc";

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}