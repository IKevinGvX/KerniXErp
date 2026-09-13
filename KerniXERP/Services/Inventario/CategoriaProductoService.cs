using KernixERP.DTOs.CategoriasProducto.Requests;
using KernixERP.DTOs.CategoriasProducto.Responses;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class CategoriaProductoService
    : ICategoriaProductoService
{
    private readonly ICategoriaProductoRepository _repository;

    public CategoriaProductoService(
        ICategoriaProductoRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<CategoriaProductoResponse>> ListarAsync(
        CategoriaProductoFiltroRequest filtro,
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

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<CategoriaProductoDetalleResponse?> ObtenerPorIdAsync(
        int idCategoriaProducto,
        CancellationToken cancellationToken)
    {
        if (idCategoriaProducto <= 0)
        {
            return Task.FromResult<
                CategoriaProductoDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idCategoriaProducto,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        CategoriaProductoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        Normalizar(request);

        return _repository.CrearAsync(
            request,
            idUsuarioRegistro,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idCategoriaProducto,
        CategoriaProductoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idCategoriaProducto <= 0)
            return Task.FromResult(false);

        Normalizar(request);

        return _repository.ActualizarAsync(
            idCategoriaProducto,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idCategoriaProducto,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idCategoriaProducto <= 0)
            return Task.FromResult(false);

        return _repository.CambiarEstadoAsync(
            idCategoriaProducto,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(
        CategoriaProductoCrearRequest request)
    {
        request.Nombre =
            request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(
                request.Descripcion);
    }

    private static void Normalizar(
        CategoriaProductoActualizarRequest request)
    {
        request.Nombre =
            request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(
                request.Descripcion);
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}