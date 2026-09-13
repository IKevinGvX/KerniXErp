using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.NotasCredito;

namespace KernixERP.Interfaces.Compras;

public interface INotaCreditoProveedorRepository
{
    Task<long> RegistrarAsync(
        int idEmpresa,
        NotaCreditoProveedorCrearRequest request,
        CancellationToken cancellationToken);

    Task<PagedResponse<NotaCreditoProveedorResponse>> ListarAsync(
        int idEmpresa,
        NotaCreditoProveedorFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<NotaCreditoProveedorDetalleResponse?> ObtenerAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AplicarAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AnularAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        string observacion,
        CancellationToken cancellationToken);
}