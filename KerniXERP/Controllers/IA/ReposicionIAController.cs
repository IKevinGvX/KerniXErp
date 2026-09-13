using KernixERP.DTOs.Common;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/reposicion")]
public sealed class ReposicionIAController : ControllerBase
{
    private readonly IReposicionIAService _service;
    private readonly ICurrentUserService _currentUser;

    public ReposicionIAController(
        IReposicionIAService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("analizar")]
    public async Task<IActionResult> Analizar(
        [FromQuery] ReposicionAnalisisRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AnalizarAsync(
                _currentUser.IdEmpresa,
                _currentUser.IdUsuario,
                request,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Análisis de reposición generado correctamente."));
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar(
        [FromBody] ReposicionAnalisisRequest request,
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
                "Predicciones y recomendaciones generadas correctamente."));
    }

    [HttpPost(
        "{idRecomendacionIA:long}/generar-solicitud")]
    public async Task<IActionResult> GenerarSolicitudCompra(
        long idRecomendacionIA,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.GenerarSolicitudCompraAsync(
                idRecomendacionIA,
                _currentUser.IdEmpresa,
                _currentUser.IdSucursal,
                _currentUser.IdEmpleado,
                _currentUser.IdUsuario,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Solicitud de compra generada desde KerniX AI."));
    }
}