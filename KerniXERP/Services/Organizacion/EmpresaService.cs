using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empresas.Requests;
using KernixERP.DTOs.Empresas.Responses;
using KernixERP.Interfaces.Organizacion;
using KernixERP.Repositories.Organizacion;

namespace KernixERP.Services.Organizacion;

public sealed class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _repository;

    public EmpresaService(
        IEmpresaRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<EmpresaResponse>> ListarAsync(
        EmpresaFiltroRequest filtro,
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
            string.IsNullOrWhiteSpace(filtro.Buscar)
                ? null
                : filtro.Buscar.Trim();

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<EmpresaDetalleResponse?> ObtenerPorIdAsync(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            return Task.FromResult<
                EmpresaDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idEmpresa,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        EmpresaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        NormalizarCrear(request);

        return _repository.CrearAsync(
            request,
            idUsuarioRegistro,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idEmpresa,
        EmpresaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            return Task.FromResult(false);
        }

        NormalizarActualizar(request);

        return _repository.ActualizarAsync(
            idEmpresa,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idEmpresa,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            return Task.FromResult(false);
        }

        return _repository.CambiarEstadoAsync(
            idEmpresa,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void NormalizarCrear(
        EmpresaCrearRequest request)
    {
        request.Ruc = request.Ruc.Trim();

        request.RazonSocial =
            request.RazonSocial.Trim();

        request.NombreComercial =
            NormalizarTexto(
                request.NombreComercial);

        request.DireccionFiscal =
            NormalizarTexto(
                request.DireccionFiscal);

        request.Telefono =
            NormalizarTexto(
                request.Telefono);

        request.Correo =
            NormalizarCorreo(
                request.Correo);

        request.SitioWeb =
            NormalizarTexto(
                request.SitioWeb);

        request.LogoUrl =
            NormalizarTexto(
                request.LogoUrl);
    }

    private static void NormalizarActualizar(
        EmpresaActualizarRequest request)
    {
        request.Ruc = request.Ruc.Trim();

        request.RazonSocial =
            request.RazonSocial.Trim();

        request.NombreComercial =
            NormalizarTexto(
                request.NombreComercial);

        request.DireccionFiscal =
            NormalizarTexto(
                request.DireccionFiscal);

        request.Telefono =
            NormalizarTexto(
                request.Telefono);

        request.Correo =
            NormalizarCorreo(
                request.Correo);

        request.SitioWeb =
            NormalizarTexto(
                request.SitioWeb);

        request.LogoUrl =
            NormalizarTexto(
                request.LogoUrl);
    }

    private static string? NormalizarTexto(
        string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }

    private static string? NormalizarCorreo(
        string? correo)
    {
        return string.IsNullOrWhiteSpace(correo)
            ? null
            : correo
                .Trim()
                .ToLowerInvariant();
    }
}