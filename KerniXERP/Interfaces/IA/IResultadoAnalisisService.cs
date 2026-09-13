// Interfaces/IA/IResultadoAnalisisService.cs

using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IResultadoAnalisisService
{
    Task<ResultadoAnalisisResponse> RegistrarAsync(
        int idUsuario,
        ResultadoAnalisisRequest request,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ResultadoAnalisisResponse>> ListarAsync(
        int idUsuario,
        ResultadoAnalisisFiltroRequest filtro,
        CancellationToken cancellationToken);
}