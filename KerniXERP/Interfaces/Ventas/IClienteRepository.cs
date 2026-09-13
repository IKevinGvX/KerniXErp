using KernixERP.DTOs.Clientes.Requests;
using KernixERP.DTOs.Clientes.Responses;
using KernixERP.DTOs.Common;

namespace KernixERP.Interfaces.Ventas;

public interface IClienteRepository
{
    Task<PagedResponse<ClienteResponse>> ListarAsync(
        ClienteFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<ClienteDetalleResponse?> ObtenerPorIdAsync(
        int idCliente,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        ClienteCrearRequest request,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idCliente,
        ClienteActualizarRequest request,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idCliente,
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken);
}