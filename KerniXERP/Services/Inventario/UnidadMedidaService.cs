using KernixERP.DTOs.Common;
using KernixERP.DTOs.UnidadesMedida.Requests;
using KernixERP.DTOs.UnidadesMedida.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class UnidadMedidaService
    : IUnidadMedidaService
{
    private readonly IUnidadMedidaRepository _repository;

    public UnidadMedidaService(
        IUnidadMedidaRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<UnidadMedidaResponse>> ListarAsync(
        UnidadMedidaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(filtro.Pagina, 1);

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

    public Task<UnidadMedidaDetalleResponse?> ObtenerPorIdAsync(
        int idUnidadMedida,
        CancellationToken cancellationToken)
    {
        if (idUnidadMedida <= 0)
        {
            return Task.FromResult<
                UnidadMedidaDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idUnidadMedida,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        UnidadMedidaCrearRequest request,
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
        int idUnidadMedida,
        UnidadMedidaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idUnidadMedida <= 0)
            return Task.FromResult(false);

        Normalizar(request);

        return _repository.ActualizarAsync(
            idUnidadMedida,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idUnidadMedida,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idUnidadMedida <= 0)
            return Task.FromResult(false);

        return _repository.CambiarEstadoAsync(
            idUnidadMedida,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(
        UnidadMedidaCrearRequest request)
    {
        request.Codigo = request.Codigo
            .Trim()
            .ToLowerInvariant();

        request.Nombre = request.Nombre.Trim();

        request.Abreviatura = request.Abreviatura
            .Trim()
            .ToLowerInvariant();
    }

    private static void Normalizar(
        UnidadMedidaActualizarRequest request)
    {
        request.Codigo = request.Codigo
            .Trim()
            .ToLowerInvariant();

        request.Nombre = request.Nombre.Trim();

        request.Abreviatura = request.Abreviatura
            .Trim()
            .ToLowerInvariant();
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}