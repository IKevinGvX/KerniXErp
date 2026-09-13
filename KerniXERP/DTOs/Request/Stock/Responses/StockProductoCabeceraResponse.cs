namespace KernixERP.DTOs.Stock.Responses;

public sealed class StockProductoCabeceraResponse
{
    public int IdProducto { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string CodigoProducto { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public string Producto { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public string AbreviaturaUnidad { get; set; } = string.Empty;

    public bool ControlaStock { get; set; }

    public bool Estado { get; set; }
}