using KernixERP.DTOs.Common;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/recomendaciones")]
public sealed class RecomendacionesIAController
    : ControllerBase
{
    private readonly IReposicionIAService _service;
    private readonly ICurrentUserService _currentUser;

    public RecomendacionesIAController(
        IReposicionIAService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? estado,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ListarRecomendacionesAsync(
                _currentUser.IdUsuario,
                estado,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Recomendaciones obtenidas correctamente."));
    }

    [HttpGet("{idRecomendacionIA:long}")]
    public async Task<IActionResult> Obtener(
        long idRecomendacionIA,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerRecomendacionPorIdAsync(
                idRecomendacionIA,
                _currentUser.IdUsuario,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "La recomendación no existe."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Recomendación obtenida correctamente."));
    }

    [HttpPatch(
        "{idRecomendacionIA:long}/descartar")]
    public async Task<IActionResult> Descartar(
        long idRecomendacionIA,
        CancellationToken cancellationToken)
    {
        await _service.DescartarRecomendacionAsync(
            idRecomendacionIA,
            _currentUser.IdUsuario,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                null,
                "Recomendación descartada correctamente."));
    }
}