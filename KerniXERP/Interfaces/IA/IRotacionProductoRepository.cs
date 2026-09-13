using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IRotacionProductoRepository
{
    Task<IReadOnlyCollection<RotacionProductoResponse>>
        AnalizarAsync(
            int idEmpresa,
            RotacionProductoRequest request,
            CancellationToken cancellationToken);
}