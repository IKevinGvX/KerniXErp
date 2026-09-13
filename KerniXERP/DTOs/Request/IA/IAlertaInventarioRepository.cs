using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IAlertaInventarioRepository
{
    Task<int> GenerarAsync(
        int idEmpresa,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AlertaInventarioResponse>> ListarAsync(
        int idEmpresa,
        AlertaInventarioFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<AlertaInventarioResponse?> ObtenerPorIdAsync(
        long idAlertaStock,
        int idEmpresa,
        CancellationToken cancellationToken);

    Task AtenderAsync(
        long idAlertaStock,
        int idEmpresa,
        CancellationToken cancellationToken);
}