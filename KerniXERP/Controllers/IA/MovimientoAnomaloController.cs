using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/anomalias-inventario")]
public sealed class MovimientoAnomaloController
    : ControllerBase
{
    private readonly IMovimientoAnomaloService _service;
    private readonly ICurrentUserService _currentUser;

    public MovimientoAnomaloController(
        IMovimientoAnomaloService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Detectar(
        [FromQuery] MovimientoAnomaloRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.DetectarAsync(
                _currentUser.IdEmpresa,
                request,
                cancellationToken);

        return Ok(resultado);
    }
    [HttpPost("generar")]
    public async Task<IActionResult> Generar(
    CancellationToken cancellationToken)
    {
        var resultado =
            await _service.GenerarAsync(
                _currentUser.IdEmpresa,
                cancellationToken);

        return Ok(resultado);
    }
    [HttpGet("registradas")]
    public async Task<IActionResult> ListarRegistradas(
    [FromQuery] AnomaliaFiltroRequest filtro,
    CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ListarAnomaliasAsync(
                _currentUser.IdEmpresa,
                filtro,
                cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("registradas/{idAnomalia:long}")]
    public async Task<IActionResult> ObtenerRegistrada(
        long idAnomalia,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerAnomaliaAsync(
                idAnomalia,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
            return NotFound();

        return Ok(resultado);
    }

    [HttpPatch("registradas/{idAnomalia:long}/atender")]
    public async Task<IActionResult> AtenderRegistrada(
        long idAnomalia,
        CancellationToken cancellationToken)
    {
        var atendida =
            await _service.AtenderAnomaliaAsync(
                idAnomalia,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (!atendida)
        {
            return BadRequest(
                new
                {
                    message =
                        "La anomalía no pudo ser atendida o ya se encontraba atendida."
                });
        }

        return Ok(
            new
            {
                idAnomalia,
                estado = "atendida"
            });
    }
}