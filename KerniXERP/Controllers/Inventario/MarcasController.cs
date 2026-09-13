using System.Security.Claims;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Marcas.Requests;
using KernixERP.Interfaces.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/marcas")]
public sealed class MarcasController : ControllerBase
{
    private readonly IMarcaService _service;

    public MarcasController(
        IMarcaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] MarcaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{idMarca:int}")]
    public async Task<IActionResult> Obtener(
        int idMarca,
        CancellationToken cancellationToken)
    {
        var result = await _service.ObtenerPorIdAsync(
            idMarca,
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Marca no encontrada."));
        }

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] MarcaCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idMarca = await _service.CrearAsync(
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idMarca },
            ApiResponse<object>.Ok(
                new { idMarca },
                "Marca creada correctamente."));
    }

    [HttpPut("{idMarca:int}")]
    public async Task<IActionResult> Actualizar(
        int idMarca,
        [FromBody] MarcaActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ActualizarAsync(
            idMarca,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Marca no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idMarca },
                "Marca actualizada correctamente."));
    }

    [HttpPatch("{idMarca:int}/activar")]
    public Task<IActionResult> Activar(
        int idMarca,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idMarca,
            true,
            cancellationToken);
    }

    [HttpPatch("{idMarca:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idMarca,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idMarca,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idMarca,
        bool estado,
        CancellationToken cancellationToken)
    {
        var result = await _service.CambiarEstadoAsync(
            idMarca,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Marca no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idMarca, estado },
                estado
                    ? "Marca activada correctamente."
                    : "Marca desactivada correctamente."));
    }

    private int ObtenerIdUsuarioActual()
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var idUsuario))
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");
        }

        return idUsuario;
    }
}