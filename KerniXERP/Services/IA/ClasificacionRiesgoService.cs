// Services/IA/ClasificacionRiesgoService.cs

using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Services.IA;

public sealed class ClasificacionRiesgoService
    : IClasificacionRiesgoService
{
    private readonly IClasificacionRiesgoRepository _repository;

    public ClasificacionRiesgoService(
        IClasificacionRiesgoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClasificacionRiesgoResponse>
        ClasificarAsync(
            ClasificacionRiesgoRequest request,
            CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.StockDisponible < 0)
        {
            throw new ArgumentException(
                "El stock disponible no puede ser negativo.");
        }

        if (request.StockMinimo < 0)
        {
            throw new ArgumentException(
                "El stock mínimo no puede ser negativo.");
        }

        var resultado =
            await _repository.ClasificarAsync(
                request,
                cancellationToken);

        return resultado
            ?? throw new InvalidOperationException(
                "No se pudo clasificar el nivel de riesgo.");
    }
}