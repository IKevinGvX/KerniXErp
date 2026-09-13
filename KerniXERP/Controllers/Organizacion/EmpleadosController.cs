using System.Security.Claims;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empleados.Requests;
using KernixERP.Interfaces.Organizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Organizacion;

[ApiController]
[Authorize]
[Route("api/v1/empleados")]
public sealed class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _service;

    public EmpleadosController(
        IEmpleadoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] EmpleadoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{idEmpleado:int}")]
    public async Task<IActionResult> Obtener(
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        var result = await _service.ObtenerPorIdAsync(
            idEmpleado,
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Empleado no encontrado."));
        }

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] EmpleadoCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idEmpleado = await _service.CrearAsync(
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idEmpleado },
            ApiResponse<object>.Ok(
                new { idEmpleado },
                "Empleado creado correctamente."));
    }

    [HttpPut("{idEmpleado:int}")]
    public async Task<IActionResult> Actualizar(
        int idEmpleado,
        [FromBody] EmpleadoActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ActualizarAsync(
            idEmpleado,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Empleado no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idEmpleado },
                "Empleado actualizado correctamente."));
    }

    [HttpPatch("{idEmpleado:int}/activar")]
    public Task<IActionResult> Activar(
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idEmpleado,
            true,
            cancellationToken);
    }

    [HttpPatch("{idEmpleado:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idEmpleado,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idEmpleado,
        bool estado,
        CancellationToken cancellationToken)
    {
        var result = await _service.CambiarEstadoAsync(
            idEmpleado,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Empleado no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idEmpleado, estado },
                estado
                    ? "Empleado activado correctamente."
                    : "Empleado desactivado correctamente."));
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