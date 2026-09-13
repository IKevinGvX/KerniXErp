using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/rotacion-productos")]
public sealed class RotacionProductoController
    : ControllerBase
{
    private readonly IRotacionProductoService _service;
    private readonly ICurrentUserService _currentUser;

    public RotacionProductoController(
        IRotacionProductoService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Analizar(
        [FromQuery] RotacionProductoRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AnalizarAsync(
                _currentUser.IdEmpresa,
                request,
                cancellationToken);

        return Ok(resultado);
    }
}