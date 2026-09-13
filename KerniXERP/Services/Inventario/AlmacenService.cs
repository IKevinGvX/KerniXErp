using KernixERP.DTOs.Almacenes.Requests;
using KernixERP.DTOs.Almacenes.Responses;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class AlmacenService : IAlmacenService
{
    private readonly IAlmacenRepository _repository;

    public AlmacenService(
        IAlmacenRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<AlmacenResponse>> ListarAsync(
        AlmacenFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(filtro.Pagina, 1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        filtro.Buscar = NormalizarTexto(filtro.Buscar);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<AlmacenDetalleResponse?> ObtenerPorIdAsync(
        int idAlmacen,
        CancellationToken cancellationToken)
    {
        if (idAlmacen <= 0)
        {
            return Task.FromResult<
                AlmacenDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idAlmacen,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        AlmacenCrearRequest request,
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
        int idAlmacen,
        AlmacenActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idAlmacen <= 0)
        {
            return Task.FromResult(false);
        }

        Normalizar(request);

        return _repository.ActualizarAsync(
            idAlmacen,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idAlmacen,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idAlmacen <= 0)
        {
            return Task.FromResult(false);
        }

        return _repository.CambiarEstadoAsync(
            idAlmacen,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(
        AlmacenCrearRequest request)
    {
        request.Codigo = request.Codigo
            .Trim()
            .ToUpperInvariant();

        request.Nombre = request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(request.Descripcion);

        request.Ubicacion =
            NormalizarTexto(request.Ubicacion);
    }

    private static void Normalizar(
        AlmacenActualizarRequest request)
    {
        request.Codigo = request.Codigo
            .Trim()
            .ToUpperInvariant();

        request.Nombre = request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(request.Descripcion);

        request.Ubicacion =
            NormalizarTexto(request.Ubicacion);
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}