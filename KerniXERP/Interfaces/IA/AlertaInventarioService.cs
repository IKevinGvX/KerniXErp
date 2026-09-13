using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Services.IA;

public sealed class AlertaInventarioService
    : IAlertaInventarioService
{
    private readonly IAlertaInventarioRepository _repository;

    public AlertaInventarioService(
        IAlertaInventarioRepository repository)
    {
        _repository = repository;
    }

    public Task<int> GenerarAsync(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        return _repository.GenerarAsync(
            idEmpresa,
            cancellationToken);
    }

    public Task<IReadOnlyCollection<AlertaInventarioResponse>> ListarAsync(
        int idEmpresa,
        AlertaInventarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        if (filtro.IdAlmacen.HasValue &&
            filtro.IdAlmacen.Value <= 0)
        {
            throw new ArgumentException(
                "El almacén no es válido.");
        }

        return _repository.ListarAsync(
            idEmpresa,
            filtro,
            cancellationToken);
    }

    public Task<AlertaInventarioResponse?> ObtenerPorIdAsync(
        long idAlertaStock,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idAlertaStock <= 0)
            throw new ArgumentException(
                "La alerta no es válida.");

        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        return _repository.ObtenerPorIdAsync(
            idAlertaStock,
            idEmpresa,
            cancellationToken);
    }

    public Task AtenderAsync(
        long idAlertaStock,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idAlertaStock <= 0)
            throw new ArgumentException(
                "La alerta no es válida.");

        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        return _repository.AtenderAsync(
            idAlertaStock,
            idEmpresa,
            cancellationToken);
    }
}