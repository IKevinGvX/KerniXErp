using KernixERP.DTOs.Common;
using KernixERP.DTOs.Roles.Responses;
using KerniXERP.DTOs.Request.Roles;

namespace KernixERP.Interfaces.Seguridad;

public interface IRolRepository
{
    Task<PagedResponse<RolResponse>> ListarAsync(
        RolFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<RolDetalleResponse?> ObtenerPorIdAsync(
        int idRol,
        CancellationToken cancellationToken);
}