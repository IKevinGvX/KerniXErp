using KernixERP.DTOs.Common;
using KernixERP.DTOs.Stock.Requests;
using KernixERP.DTOs.Stock.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class StockService : IStockService
{
    private static readonly HashSet<string> EstadosStockPermitidos =
    [
        "SIN_STOCK",
        "STOCK_BAJO",
        "NORMAL",
        "SOBRE_STOCK"
    ];

    private readonly IStockRepository _repository;

    public StockService(
        IStockRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<StockResponse>> ListarAsync(
        StockFiltroRequest filtro,
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
                .ToUpperInvariant();

        if (filtro.EstadoStock is not null &&
            !EstadosStockPermitidos.Contains(
                filtro.EstadoStock))
        {
            throw new ArgumentException(
                "El estado de stock no es válido. Valores permitidos: SIN_STOCK, STOCK_BAJO, NORMAL y SOBRE_STOCK.");
        }

        ValidarIdOpcional(
            filtro.IdEmpresa,
            "empresa");

        ValidarIdOpcional(
            filtro.IdSucursal,
            "sucursal");

        ValidarIdOpcional(
            filtro.IdAlmacen,
            "almacén");

        ValidarIdOpcional(
            filtro.IdCategoriaProducto,
            "categoría");

        ValidarIdOpcional(
            filtro.IdMarca,
            "marca");

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<StockDetalleResponse?> ObtenerPorIdAsync(
        int idStock,
        CancellationToken cancellationToken)
    {
        if (idStock <= 0)
        {
            return Task.FromResult<
                StockDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idStock,
            cancellationToken);
    }

    public Task<StockProductoDetalleResponse?> ObtenerPorProductoAsync(
        int idProducto,
        int? idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idProducto <= 0)
        {
            return Task.FromResult<
                StockProductoDetalleResponse?>(null);
        }

        ValidarIdOpcional(
            idEmpresa,
            "empresa");

        return _repository.ObtenerPorProductoAsync(
            idProducto,
            idEmpresa,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idStock,
        StockActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idStock <= 0)
        {
            return Task.FromResult(false);
        }

        if (idUsuarioModifica <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");
        }

        ValidarLimites(
            request.StockMinimo,
            request.StockMaximo);

        return _repository.ActualizarAsync(
            idStock,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<StockDashboardResponse> ObtenerDashboardAsync(
        int idEmpresa,
        int? idSucursal,
        int? idAlmacen,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        ValidarIdOpcional(
            idSucursal,
            "sucursal");

        ValidarIdOpcional(
            idAlmacen,
            "almacén");

        return _repository.ObtenerDashboardAsync(
            idEmpresa,
            idSucursal,
            idAlmacen,
            cancellationToken);
    }

    private static void ValidarLimites(
        decimal stockMinimo,
        decimal? stockMaximo)
    {
        if (stockMinimo < 0)
        {
            throw new ArgumentException(
                "El stock mínimo no puede ser negativo.");
        }

        if (stockMaximo.HasValue &&
            stockMaximo.Value < 0)
        {
            throw new ArgumentException(
                "El stock máximo no puede ser negativo.");
        }

        if (stockMaximo.HasValue &&
            stockMaximo.Value < stockMinimo)
        {
            throw new ArgumentException(
                "El stock máximo no puede ser menor al stock mínimo.");
        }
    }

    private static void ValidarIdOpcional(
        int? id,
        string nombre)
    {
        if (id.HasValue &&
            id.Value <= 0)
        {
            throw new ArgumentException(
                $"El identificador de {nombre} no es válido.");
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