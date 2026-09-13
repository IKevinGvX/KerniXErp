// Interfaces/IA/IClasificacionRiesgoService.cs

using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IClasificacionRiesgoService
{
    Task<ClasificacionRiesgoResponse>
        ClasificarAsync(
            ClasificacionRiesgoRequest request,
            CancellationToken cancellationToken);
}