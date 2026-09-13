// Interfaces/IA/IResultadoAnalisisRepository.cs

using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IResultadoAnalisisRepository
{
    Task<ResultadoAnalisisResponse?> RegistrarAsync(
        int idUsuario,
        ResultadoAnalisisRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ResultadoAnalisisResponse>> ListarAsync(
        int idUsuario,
        ResultadoAnalisisFiltroRequest filtro,
        CancellationToken cancellationToken);
}