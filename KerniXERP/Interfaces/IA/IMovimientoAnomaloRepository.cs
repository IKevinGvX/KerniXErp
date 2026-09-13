using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IMovimientoAnomaloRepository
{
    Task<IReadOnlyCollection<MovimientoAnomaloResponse>>
        DetectarAsync(
            int idEmpresa,
            MovimientoAnomaloRequest request,
            CancellationToken cancellationToken);

    Task<long> RegistrarAnomaliaAsync(
        int idModeloIA,
        MovimientoAnomaloResponse movimiento,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AnomaliaRegistradaResponse>>
        ListarAnomaliasAsync(
            int idEmpresa,
            AnomaliaFiltroRequest filtro,
            CancellationToken cancellationToken);

    Task<AnomaliaDetalleResponse?> ObtenerAnomaliaAsync(
        long idAnomalia,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<bool> AtenderAnomaliaAsync(
        long idAnomalia,
        int idEmpresa,
        CancellationToken cancellationToken);
}