// Interfaces/IA/IClasificacionRiesgoRepository.cs

using KernixERP.DTOs.IA;

namespace KernixERP.Interfaces.IA;

public interface IClasificacionRiesgoRepository
{
    Task<ClasificacionRiesgoResponse?> ClasificarAsync(
        ClasificacionRiesgoRequest request,
        CancellationToken cancellationToken);
}