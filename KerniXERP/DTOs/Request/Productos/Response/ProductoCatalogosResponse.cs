namespace KernixERP.DTOs.Productos.Responses;

public sealed class ProductoCatalogosResponse
{
    public List<CatalogoItemResponse> Categorias { get; set; } = [];

    public List<CatalogoItemResponse> Marcas { get; set; } = [];

    public List<CatalogoItemResponse> UnidadesMedida { get; set; } = [];

    public List<CatalogoItemResponse> Sucursales { get; set; } = [];

    public List<CatalogoItemResponse> Almacenes { get; set; } = [];
}