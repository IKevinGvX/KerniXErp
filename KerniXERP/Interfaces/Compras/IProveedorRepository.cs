using KernixERP.DTOs.Proveedores.Requests;
using KernixERP.DTOs.Proveedores.Responses;

namespace KernixERP.Interfaces.Compras;

public interface IProveedorRepository
{
    Task<IReadOnlyCollection<ProveedorResponse>> ListarAsync(
        ProveedorFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<ProveedorDetalleResponse?> ObtenerPorIdAsync(
        int idProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        ProveedorCrearRequest request,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idProveedor,
        int idEmpresa,
        ProveedorActualizarRequest request,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idProveedor,
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken);
}