namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockDetalleResponse
{
    public int IdStock { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public int IdSucursal { get; set; }

    public string Sucursal { get; set; } = string.Empty;

    public int IdAlmacen { get; set; }

    public string CodigoAlmacen { get; set; } = string.Empty;

    public string Almacen { get; set; } = string.Empty;

    public string? UbicacionAlmacen { get; set; }

    public int IdProducto { get; set; }

    public string CodigoProducto { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string? DescripcionProducto { get; set; }

    public int IdCategoriaProducto { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public int IdMarca { get; set; }

    public string Marca { get; set; } = string.Empty;

    public int IdUnidadMedida { get; set; }

    public string UnidadMedida { get; set; } = string.Empty;

    public string AbreviaturaUnidad { get; set; } = string.Empty;

    public decimal CantidadActual { get; set; }

    public decimal CantidadReservada { get; set; }

    public decimal CantidadDisponible { get; set; }

    public decimal StockMinimo { get; set; }

    public decimal? StockMaximo { get; set; }

    public string EstadoStock { get; set; } = string.Empty;

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public decimal ValorCompra { get; set; }

    public decimal ValorVenta { get; set; }

    public DateTime FechaActualizacion { get; set; }
}