namespace KernixERP.DTOs.Stock.Requests;

public sealed class StockFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdAlmacen { get; set; }

    public int? IdCategoriaProducto { get; set; }

    public int? IdMarca { get; set; }

    public string? EstadoStock { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}