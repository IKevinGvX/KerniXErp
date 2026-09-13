using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras;

namespace KernixERP.Interfaces.Compras;

public interface IRecepcionCompraRepository
{
    Task<PagedResponse<RecepcionCompraResponse>> ListarAsync(
        RecepcionCompraFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<RecepcionCompraDetalleResponse?> ObtenerPorIdAsync(
        long idRecepcionCompra,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<(long IdRecepcionCompra, string NumeroRecepcion)> RegistrarAsync(
        RecepcionCompraCrearRequest request,
        int idEmpresa,
        int idEmpleado,
        CancellationToken cancellationToken);

    Task<bool> AnularAsync(
        long idRecepcionCompra,
        int idEmpresa,
        int idEmpleado,
        string motivo,
        CancellationToken cancellationToken);
}