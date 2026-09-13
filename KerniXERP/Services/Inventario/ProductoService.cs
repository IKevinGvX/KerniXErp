using KernixERP.DTOs.Common;
using KernixERP.DTOs.Productos.Requests;
using KernixERP.DTOs.Productos.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class ProductoService : IProductoService
{
    private static readonly HashSet<string> EstadosStockPermitidos =
    [
        "sin_stock",
        "stock_bajo",
        "normal",
        "sobre_stock",
        "no_controlado"
    ];

    private static readonly HashSet<string> CamposOrdenPermitidos =
    [
        "codigo",
        "nombre",
        "categoria",
        "marca",
        "precio_compra",
        "precio_venta",
        "stock",
        "fecha_registro"
    ];

    private readonly IProductoRepository _repository;

    public ProductoService(
        IProductoRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<ProductoResponse>> ListarAsync(
        ProductoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(
            filtro.Pagina,
            1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        filtro.Buscar =
            NormalizarTexto(filtro.Buscar);

        filtro.EstadoStock =
            NormalizarTexto(filtro.EstadoStock)?
                .ToLowerInvariant();

        filtro.OrdenarPor =
            NormalizarTexto(filtro.OrdenarPor)?
                .ToLowerInvariant()
            ?? "nombre";

        filtro.DireccionOrden =
            NormalizarTexto(filtro.DireccionOrden)?
                .ToLowerInvariant()
            ?? "asc";

        if (filtro.EstadoStock is not null &&
            !EstadosStockPermitidos.Contains(
                filtro.EstadoStock))
        {
            throw new ArgumentException(
                "El estado de stock no es válido.");
        }

        if (!CamposOrdenPermitidos.Contains(
                filtro.OrdenarPor))
        {
            filtro.OrdenarPor = "nombre";
        }

        if (filtro.DireccionOrden is not
            ("asc" or "desc"))
        {
            filtro.DireccionOrden = "asc";
        }

        if (filtro.PrecioDesde.HasValue &&
            filtro.PrecioDesde.Value < 0)
        {
            throw new ArgumentException(
                "El precio mínimo no puede ser negativo.");
        }

        if (filtro.PrecioHasta.HasValue &&
            filtro.PrecioHasta.Value < 0)
        {
            throw new ArgumentException(
                "El precio máximo no puede ser negativo.");
        }

        if (filtro.PrecioDesde.HasValue &&
            filtro.PrecioHasta.HasValue &&
            filtro.PrecioHasta.Value <
            filtro.PrecioDesde.Value)
        {
            throw new ArgumentException(
                "El precio máximo no puede ser menor al precio mínimo.");
        }

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<ProductoDetalleResponse?> ObtenerPorIdAsync(
        int idProducto,
        CancellationToken cancellationToken)
    {
        if (idProducto <= 0)
        {
            return Task.FromResult<
                ProductoDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idProducto,
            cancellationToken);
    }

    public Task<ProductoCatalogosResponse> ObtenerCatalogosAsync(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        return _repository.ObtenerCatalogosAsync(
            idEmpresa,
            cancellationToken);
    }

    public Task<ProductoBusquedaResponse?> BuscarCodigoAsync(
        int idEmpresa,
        string codigo,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        codigo = NormalizarTexto(codigo)
            ?? throw new ArgumentException(
                "El código es obligatorio.");

        return _repository.BuscarCodigoAsync(
            idEmpresa,
            codigo,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        ProductoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        Normalizar(request);
        ValidarDatos(
            request.PrecioCompra,
            request.PrecioVenta,
            request.StockMinimo,
            request.StockMaximo);

        return _repository.CrearAsync(
            request,
            idUsuarioRegistro,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idProducto,
        ProductoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idProducto <= 0)
        {
            return Task.FromResult(false);
        }

        Normalizar(request);
        ValidarDatos(
            request.PrecioCompra,
            request.PrecioVenta,
            request.StockMinimo,
            request.StockMaximo);

        return _repository.ActualizarAsync(
            idProducto,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idProducto,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idProducto <= 0)
        {
            return Task.FromResult(false);
        }

        return _repository.CambiarEstadoAsync(
            idProducto,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(
        ProductoCrearRequest request)
    {
        request.Codigo = request.Codigo
            .Trim()
            .ToUpperInvariant();

        request.CodigoBarras =
            NormalizarTexto(request.CodigoBarras);

        request.Nombre =
            request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(request.Descripcion);
    }

    private static void Normalizar(
        ProductoActualizarRequest request)
    {
        request.Codigo = request.Codigo
            .Trim()
            .ToUpperInvariant();

        request.CodigoBarras =
            NormalizarTexto(request.CodigoBarras);

        request.Nombre =
            request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(request.Descripcion);
    }

    private static void ValidarDatos(
        decimal precioCompra,
        decimal precioVenta,
        decimal stockMinimo,
        decimal? stockMaximo)
    {
        if (precioCompra < 0 ||
            precioVenta < 0)
        {
            throw new ArgumentException(
                "Los precios no pueden ser negativos.");
        }

        if (precioVenta < precioCompra)
        {
            throw new ArgumentException(
                "El precio de venta no puede ser menor al precio de compra.");
        }

        if (stockMinimo < 0)
        {
            throw new ArgumentException(
                "El stock mínimo no puede ser negativo.");
        }

        if (stockMaximo.HasValue &&
            stockMaximo.Value < stockMinimo)
        {
            throw new ArgumentException(
                "El stock máximo no puede ser menor al stock mínimo.");
        }
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}