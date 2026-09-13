using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.DocumentosProveedor;

namespace KernixERP.Interfaces.Compras;

public interface IDocumentoProveedorService
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

    Task AplicarAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AnularAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        DocumentoProveedorAnularRequest request,
        CancellationToken cancellationToken);
}