using System.Security.Claims;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Sucursales.Requests;
using KernixERP.Interfaces.Organizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Organizacion;

[ApiController]
[Authorize]
[Route("api/v1/sucursales")]
public sealed class SucursalesController : ControllerBase
{
    private readonly ISucursalService _service;

    public SucursalesController(ISucursalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] SucursalFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{idSucursal:int}")]
    public async Task<IActionResult> Obtener(
        int idSucursal,
        CancellationToken cancellationToken)
    {
        var result = await _service.ObtenerPorIdAsync(
            idSucursal,
            cancellationToken);

        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Sucursal no encontrada."));

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] SucursalCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idSucursal = await _service.CrearAsync(
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idSucursal },
            ApiResponse<object>.Ok(
                new { idSucursal },
                "Sucursal creada correctamente."));
    }

    [HttpPut("{idSucursal:int}")]
    public async Task<IActionResult> Actualizar(
        int idSucursal,
        [FromBody] SucursalActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ActualizarAsync(
            idSucursal,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
            return NotFound(ApiResponse<object>.Fail("Sucursal no encontrada."));

        return Ok(ApiResponse<object>.Ok(
            new { idSucursal },
            "Sucursal actualizada correctamente."));
    }

    [HttpPatch("{idSucursal:int}/activar")]
    public Task<IActionResult> Activar(
        int idSucursal,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(idSucursal, true, cancellationToken);
    }

    [HttpPatch("{idSucursal:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idSucursal,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(idSucursal, false, cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idSucursal,
        bool estado,
        CancellationToken cancellationToken)
    {
        var result = await _service.CambiarEstadoAsync(
            idSucursal,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
            return NotFound(ApiResponse<object>.Fail("Sucursal no encontrada."));

        return Ok(ApiResponse<object>.Ok(
            new { idSucursal, estado },
            estado
                ? "Sucursal activada correctamente."
                : "Sucursal desactivada correctamente."));
    }

    private int ObtenerIdUsuarioActual()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var idUsuario))
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");

        return idUsuario;
    }
}