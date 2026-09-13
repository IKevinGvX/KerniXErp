using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IReposicionIAService
{
    Task<IReadOnlyCollection<RecomendacionIAResponse>>
    ListarRecomendacionesAsync(
        int idUsuario,
        string? estado,
        CancellationToken cancellationToken);

    Task<RecomendacionIAResponse?>
        ObtenerRecomendacionPorIdAsync(
            long idRecomendacionIA,
            int idUsuario,
            CancellationToken cancellationToken);

    Task DescartarRecomendacionAsync(
        long idRecomendacionIA,
        int idUsuario,
        CancellationToken cancellationToken);
    Task<IReadOnlyCollection<RecomendacionReposicionResponse>>
        AnalizarAsync(
            int idEmpresa,
            int idUsuario,
            ReposicionAnalisisRequest request,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ReposicionGeneradaResponse>>
        GenerarAsync(
            int idEmpresa,
            int idUsuario,
            ReposicionAnalisisRequest request,
            CancellationToken cancellationToken);

    Task<SolicitudCompraDesdeIAResponse>
        GenerarSolicitudCompraAsync(
            long idRecomendacionIA,
            int idEmpresa,
            int idSucursal,
            int idEmpleado,
            int idUsuario,
            CancellationToken cancellationToken);
}