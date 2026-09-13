using KerniXERP.DTOs.Request.Productos.Response;

namespace KernixERP.DTOs.Productos.Responses;

public sealed class ProductoDetalleResponse
{
    public int IdProducto { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public int IdCategoriaProducto { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public int IdMarca { get; set; }

    public string Marca { get; set; } = string.Empty;

    public int IdUnidadMedida { get; set; }

    public string UnidadMedida { get; set; } = string.Empty;

    public string AbreviaturaUnidad { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public decimal MargenMonto { get; set; }

    public decimal MargenPorcentaje { get; set; }

    public decimal StockMinimo { get; set; }

    public decimal? StockMaximo { get; set; }

    public bool ControlaStock { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public List<ProductoStockResponse> StockPorAlmacen { get; set; } = [];

    public ProductoIndicadoresResponse Indicadores { get; set; } = new();
}