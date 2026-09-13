using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empresas.Requests;
using KernixERP.DTOs.Empresas.Responses;
using KernixERP.Interfaces.Organizacion;

namespace KernixERP.Repositories.Organizacion;

public interface IEmpresaRepository
{
    Task<PagedResponse<EmpresaResponse>> ListarAsync(
        EmpresaFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<EmpresaDetalleResponse?> ObtenerPorIdAsync(
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        EmpresaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idEmpresa,
        EmpresaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idEmpresa,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}