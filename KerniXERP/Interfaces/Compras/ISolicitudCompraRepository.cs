using KernixERP.DTOs.Common;
using KernixERP.DTOs.SolicitudesCompra.Requests;
using KernixERP.DTOs.SolicitudesCompra.Responses;

namespace KernixERP.Interfaces.Compras;

public interface ISolicitudCompraRepository
{
    Task<PagedResponse<SolicitudCompraResponse>> ListarAsync(
        SolicitudCompraFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<SolicitudCompraDetalleResponse?> ObtenerPorIdAsync(
        long idSolicitudCompra,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<(long IdSolicitudCompra, string NumeroSolicitud)> CrearAsync(
        SolicitudCompraCrearRequest request,
        int idEmpleadoSolicita,
        CancellationToken cancellationToken);

    Task<bool> AprobarAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleadoAprueba,
        SolicitudCompraAprobarRequest request,
        CancellationToken cancellationToken);

    Task<bool> RechazarAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleadoAprueba,
        string observacion,
        CancellationToken cancellationToken);

    Task<bool> AnularAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleado,
        string observacion,
        CancellationToken cancellationToken);
}