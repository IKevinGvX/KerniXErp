using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IMovimientoAnomaloService
{
    Task<IReadOnlyCollection<MovimientoAnomaloResponse>>
        DetectarAsync(
            int idEmpresa,
            MovimientoAnomaloRequest request,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AnomaliaGeneradaResponse>>
        GenerarAsync(
            int idEmpresa,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AnomaliaRegistradaResponse>>
        ListarAnomaliasAsync(
            int idEmpresa,
            AnomaliaFiltroRequest filtro,
            CancellationToken cancellationToken);

    Task<AnomaliaDetalleResponse?>
        ObtenerAnomaliaAsync(
            long idAnomalia,
            int idEmpresa,
            CancellationToken cancellationToken);

    Task<bool> AtenderAnomaliaAsync(
        long idAnomalia,
        int idEmpresa,
        CancellationToken cancellationToken);
}