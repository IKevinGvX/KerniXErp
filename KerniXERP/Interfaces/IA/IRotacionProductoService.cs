using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IRotacionProductoService
{
    Task<IReadOnlyCollection<RotacionProductoResponse>>
        AnalizarAsync(
            int idEmpresa,
            RotacionProductoRequest request,
            CancellationToken cancellationToken);
}