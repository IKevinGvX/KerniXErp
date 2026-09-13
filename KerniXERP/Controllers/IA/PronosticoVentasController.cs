using KernixERP.DTOs.Common;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/pronostico-ventas")]
public sealed class PronosticoVentasController
    : ControllerBase
{
    private readonly IPronosticoVentasService _service;
    private readonly ICurrentUserService _currentUser;

    public PronosticoVentasController(
        IPronosticoVentasService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Analizar(
        [FromQuery] PronosticoVentaRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AnalizarAsync(
                _currentUser.IdEmpresa,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Pronóstico de ventas obtenido correctamente."));
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar(
        [FromBody] PronosticoVentaRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.GenerarAsync(
                _currentUser.IdEmpresa,
                _currentUser.IdUsuario,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Pronóstico de ventas generado y registrado correctamente."));
    }

    [HttpGet("predicciones")]
    public async Task<IActionResult> ListarPredicciones(
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ListarPrediccionesAsync(
                _currentUser.IdUsuario,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Predicciones de ventas obtenidas correctamente."));
    }
}