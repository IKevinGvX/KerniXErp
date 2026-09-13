// Controllers/IA/ClasificacionRiesgoController.cs

using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/clasificacion-riesgo")]
public sealed class ClasificacionRiesgoController
    : ControllerBase
{
    private readonly IClasificacionRiesgoService _service;

    public ClasificacionRiesgoController(
        IClasificacionRiesgoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Clasificar(
        [FromBody] ClasificacionRiesgoRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ClasificarAsync(
                request,
                cancellationToken);

        return Ok(resultado);
    }
}