using KernixERP.DTOs.IA;
using KernixERP.Models.IA;

namespace KernixERP.Interfaces.IA;

public interface IPronosticoVentasRepository
{
    Task<IReadOnlyCollection<PronosticoVentaResponse>>
        AnalizarAsync(
            int idEmpresa,
            PronosticoVentaRequest request,
            CancellationToken cancellationToken);

    Task<long> GuardarPrediccionAsync(
        int idModeloIA,
        int idUsuario,
        PronosticoVentaResponse pronostico,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PrediccionIA>>
        ListarPrediccionesAsync(
            int idUsuario,
            CancellationToken cancellationToken);
}