using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.DocumentosProveedor;

namespace KernixERP.Interfaces.Compras;

public interface IDocumentoProveedorRepository
{
    Task<long> RegistrarAsync(
        int idEmpresa,
        DocumentoProveedorCrearRequest request,
        CancellationToken cancellationToken);

    Task<PagedResponse<DocumentoProveedorResponse>> ListarAsync(
        int idEmpresa,
        DocumentoProveedorFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<DocumentoProveedorDetalleResponse?> ObtenerAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AplicarNotaCreditoAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AplicarNotaDebitoAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AnularNotaCreditoAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        string observacion,
        CancellationToken cancellationToken);

    Task AnularNotaDebitoAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        string observacion,
        CancellationToken cancellationToken);
}