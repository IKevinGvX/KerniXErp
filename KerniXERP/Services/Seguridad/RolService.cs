using KernixERP.DTOs.Common;
using KernixERP.DTOs.Roles.Responses;
using KernixERP.Interfaces.Seguridad;
using KerniXERP.DTOs.Request.Roles;

namespace KernixERP.Services.Seguridad;

public sealed class RolService : IRolService
{
    private readonly IRolRepository _repository;

    public RolService(IRolRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<RolResponse>> ListarAsync(
        RolFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(filtro.Pagina, 1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<RolDetalleResponse?> ObtenerPorIdAsync(
        int idRol,
        CancellationToken cancellationToken)
    {
        return _repository.ObtenerPorIdAsync(
            idRol,
            cancellationToken);
    }
}