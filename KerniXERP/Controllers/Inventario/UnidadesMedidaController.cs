using System.Security.Claims;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.UnidadesMedida.Requests;
using KernixERP.Interfaces.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/unidades-medida")]
public sealed class UnidadesMedidaController
    : ControllerBase
{
    private readonly IUnidadMedidaService _service;

    public UnidadesMedidaController(
        IUnidadMedidaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] UnidadMedidaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(result));
    }

    [HttpGet("{idUnidadMedida:int}")]
    public async Task<IActionResult> Obtener(
        int idUnidadMedida,
        CancellationToken cancellationToken)
    {
        var result = await _service.ObtenerPorIdAsync(
            idUnidadMedida,
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Unidad de medida no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] UnidadMedidaCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idUnidadMedida =
            await _service.CrearAsync(
                request,
                ObtenerIdUsuarioActual(),
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idUnidadMedida },
            ApiResponse<object>.Ok(
                new { idUnidadMedida },
                "Unidad de medida creada correctamente."));
    }

    [HttpPut("{idUnidadMedida:int}")]
    public async Task<IActionResult> Actualizar(
        int idUnidadMedida,
        [FromBody] UnidadMedidaActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ActualizarAsync(
            idUnidadMedida,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Unidad de medida no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idUnidadMedida },
                "Unidad de medida actualizada correctamente."));
    }

    [HttpPatch("{idUnidadMedida:int}/activar")]
    public Task<IActionResult> Activar(
        int idUnidadMedida,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idUnidadMedida,
            true,
            cancellationToken);
    }

    [HttpPatch("{idUnidadMedida:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idUnidadMedida,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idUnidadMedida,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idUnidadMedida,
        bool estado,
        CancellationToken cancellationToken)
    {
        var result = await _service.CambiarEstadoAsync(
            idUnidadMedida,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Unidad de medida no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idUnidadMedida,
                    estado
                },
                estado
                    ? "Unidad de medida activada correctamente."
                    : "Unidad de medida desactivada correctamente."));
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