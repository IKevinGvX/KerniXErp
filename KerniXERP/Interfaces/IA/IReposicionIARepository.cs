using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IReposicionIARepository
{
    Task<IReadOnlyCollection<RecomendacionReposicionResponse>>
        AnalizarAsync(
            int idEmpresa,
            int idUsuario,
            ReposicionAnalisisRequest request,
            CancellationToken cancellationToken);
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
    Task<long> GuardarPrediccionAsync(
        int idUsuario,
        RecomendacionReposicionResponse analisis,
        int diasHistorial,
        int diasProyeccion,
        CancellationToken cancellationToken);

    Task<long> GuardarRecomendacionAsync(
        int idUsuario,
        RecomendacionReposicionResponse analisis,
        CancellationToken cancellationToken);

    Task<RecomendacionReposicionDetalleResponse?>
        ObtenerRecomendacionAsync(
            long idRecomendacionIA,
            int idUsuario,
            CancellationToken cancellationToken);

    Task ProcesarRecomendacionAsync(
        long idRecomendacionIA,
        int idUsuario,
        CancellationToken cancellationToken);
}