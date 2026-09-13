using KernixERP.DTOs.Common;
using KernixERP.DTOs.Sucursales.Requests;
using KernixERP.DTOs.Sucursales.Responses;
using KernixERP.Interfaces.Organizacion;

namespace KernixERP.Services.Organizacion;

public sealed class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _repository;

    public SucursalService(ISucursalRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<SucursalResponse>> ListarAsync(
        SucursalFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(filtro.Pagina, 1);
        filtro.TamanoPagina = Math.Clamp(filtro.TamanoPagina, 1, 100);
        filtro.Buscar = NormalizarTexto(filtro.Buscar);

        return _repository.ListarAsync(filtro, cancellationToken);
    }

    public Task<SucursalDetalleResponse?> ObtenerPorIdAsync(
        int idSucursal,
        CancellationToken cancellationToken)
    {
        if (idSucursal <= 0)
            return Task.FromResult<SucursalDetalleResponse?>(null);

        return _repository.ObtenerPorIdAsync(
            idSucursal,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        SucursalCrearRequest request,
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
        int idSucursal,
        SucursalActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idSucursal <= 0)
            return Task.FromResult(false);

        Normalizar(request);

        return _repository.ActualizarAsync(
            idSucursal,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idSucursal,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idSucursal <= 0)
            return Task.FromResult(false);

        return _repository.CambiarEstadoAsync(
            idSucursal,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(SucursalCrearRequest request)
    {
        request.Codigo = request.Codigo.Trim().ToUpperInvariant();
        request.Nombre = request.Nombre.Trim();
        request.Direccion = NormalizarTexto(request.Direccion);
        request.Telefono = NormalizarTexto(request.Telefono);
        request.Correo = NormalizarCorreo(request.Correo);
        request.Ubigeo = NormalizarTexto(request.Ubigeo);
    }

    private static void Normalizar(SucursalActualizarRequest request)
    {
        request.Codigo = request.Codigo.Trim().ToUpperInvariant();
        request.Nombre = request.Nombre.Trim();
        request.Direccion = NormalizarTexto(request.Direccion);
        request.Telefono = NormalizarTexto(request.Telefono);
        request.Correo = NormalizarCorreo(request.Correo);
        request.Ubigeo = NormalizarTexto(request.Ubigeo);
    }

    private static string? NormalizarTexto(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string? NormalizarCorreo(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();
    }
}