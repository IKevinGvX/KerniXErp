using KernixERP.DTOs.Common;
using KernixERP.DTOs.Marcas.Requests;
using KernixERP.DTOs.Marcas.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class MarcaService : IMarcaService
{
    private readonly IMarcaRepository _repository;

    public MarcaService(
        IMarcaRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<MarcaResponse>> ListarAsync(
        MarcaFiltroRequest filtro,
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

    public Task<MarcaDetalleResponse?> ObtenerPorIdAsync(
        int idMarca,
        CancellationToken cancellationToken)
    {
        if (idMarca <= 0)
        {
            return Task.FromResult<
                MarcaDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idMarca,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        MarcaCrearRequest request,
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
        int idMarca,
        MarcaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idMarca <= 0)
            return Task.FromResult(false);

        Normalizar(request);

        return _repository.ActualizarAsync(
            idMarca,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idMarca,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idMarca <= 0)
            return Task.FromResult(false);

        return _repository.CambiarEstadoAsync(
            idMarca,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(
        MarcaCrearRequest request)
    {
        request.Nombre = request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(request.Descripcion);
    }

    private static void Normalizar(
        MarcaActualizarRequest request)
    {
        request.Nombre = request.Nombre.Trim();

        request.Descripcion =
            NormalizarTexto(request.Descripcion);
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}