using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Seguridad;
using KerniXERP.DTOs.Request.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Seguridad;

[ApiController]
[Authorize]
[Route("api/v1/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IRolService _service;

    public RolesController(IRolService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] RolFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(resultado));
    }

    [HttpGet("{idRol:int}")]
    public async Task<IActionResult> Obtener(
        int idRol,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idRol,
            cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Rol no encontrado."));
        }

        return Ok(ApiResponse<object>.Ok(resultado));
    }
}