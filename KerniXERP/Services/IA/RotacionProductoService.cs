using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Services.IA;

public sealed class RotacionProductoService
    : IRotacionProductoService
{
    private readonly IRotacionProductoRepository _repository;

    public RotacionProductoService(
        IRotacionProductoRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<RotacionProductoResponse>>
        AnalizarAsync(
            int idEmpresa,
            RotacionProductoRequest request,
            CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa no es válida.");
        }

        ArgumentNullException.ThrowIfNull(request);

        if (request.IdAlmacen.HasValue &&
            request.IdAlmacen.Value <= 0)
        {
            throw new ArgumentException(
                "El almacén no es válido.");
        }

        if (request.DiasHistorial < 7 ||
            request.DiasHistorial > 365)
        {
            throw new ArgumentException(
                "Los días de historial deben estar entre 7 y 365.");
        }

        return _repository.AnalizarAsync(
            idEmpresa,
            request,
            cancellationToken);
    }
}