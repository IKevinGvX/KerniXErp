using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empleados.Requests;
using KernixERP.DTOs.Empleados.Responses;

namespace KernixERP.Interfaces.Organizacion;

public interface IEmpleadoRepository
{
    Task<PagedResponse<EmpleadoResponse>> ListarAsync(
        EmpleadoFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<EmpleadoDetalleResponse?> ObtenerPorIdAsync(
        int idEmpleado,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        EmpleadoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idEmpleado,
        EmpleadoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idEmpleado,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}