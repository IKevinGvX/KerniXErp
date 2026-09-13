using KernixERP.DTOs.IA;
using KernixERP.Models.IA;

namespace KernixERP.Interfaces.IA;

public interface IPronosticoVentasService
{
    Task<IReadOnlyCollection<PronosticoVentaResponse>>
        AnalizarAsync(
            int idEmpresa,
            PronosticoVentaRequest request,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PronosticoVentaGeneradoResponse>>
        GenerarAsync(
            int idEmpresa,
            int idUsuario,
            PronosticoVentaRequest request,
            CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PrediccionIA>>
        ListarPrediccionesAsync(
            int idUsuario,
            CancellationToken cancellationToken);
}