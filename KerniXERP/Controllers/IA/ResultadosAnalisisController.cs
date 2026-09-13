// Controllers/IA/ResultadosAnalisisController.cs

using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/resultados-analisis")]
public sealed class ResultadosAnalisisController
    : ControllerBase
{
    private readonly IResultadoAnalisisService _service;
    private readonly ICurrentUserService _currentUser;

    public ResultadosAnalisisController(
        IResultadoAnalisisService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] ResultadoAnalisisRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.RegistrarAsync(
                _currentUser.IdUsuario,
                request,
                cancellationToken);

        return Ok(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ResultadoAnalisisFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ListarAsync(
                _currentUser.IdUsuario,
                filtro,
                cancellationToken);

        return Ok(resultado);
    }
}