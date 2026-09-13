using KernixERP.DTOs.Common;
using KernixERP.DTOs.Usuarios.Requests;
using KernixERP.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KernixERP.Repositories.Seguridad;
using KernixERP.Services.Seguridad;
namespace KernixERP.Controllers.Seguridad;

[ApiController]
[Authorize]
[Route("api/v1/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{idUsuario:int}")]
    public async Task<IActionResult> Obtener(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        var result = await _service.ObtenerPorIdAsync(
            idUsuario,
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Usuario no encontrado."));
        }

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] UsuarioCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idUsuarioActual = ObtenerIdUsuarioActual();

        var idUsuario = await _service.CrearAsync(
            request,
            idUsuarioActual,
            cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idUsuario },
            ApiResponse<object>.Ok(
                new { idUsuario },
                "Usuario creado correctamente."));
    }

    [HttpPut("{idUsuario:int}")]
    public async Task<IActionResult> Actualizar(
        int idUsuario,
        [FromBody] UsuarioActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var actualizado = await _service.ActualizarAsync(
            idUsuario,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!actualizado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Usuario no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idUsuario },
                "Usuario actualizado correctamente."));
    }

    [HttpPatch("{idUsuario:int}/activar")]
    public Task<IActionResult> Activar(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idUsuario,
            true,
            cancellationToken);
    }

    [HttpPatch("{idUsuario:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idUsuario,
            false,
            cancellationToken);
    }

    [HttpPatch("{idUsuario:int}/cambiar-password")]
    public async Task<IActionResult> CambiarPassword(
    int idUsuario,
    [FromBody] UsuarioCambiarPasswordRequest request,
    CancellationToken cancellationToken)
    {
        var idUsuarioActual = ObtenerIdUsuarioActual();

        if (idUsuario != idUsuarioActual)
        {
            return Forbid();
        }

        var result = await _service.CambiarPasswordAsync(
            idUsuario,
            request,
            cancellationToken);

        if (!result)
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    "La contraseña actual es incorrecta o la nueva contraseña no es válida."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idUsuario },
                "Contraseña actualizada correctamente. Inicia sesión nuevamente."));
    }

    [HttpPut("{idUsuario:int}/roles")]
    public async Task<IActionResult> AsignarRoles(
        int idUsuario,
        [FromBody] UsuarioRolesRequest request,
        CancellationToken cancellationToken)
    {
        await _service.AsignarRolesAsync(
            idUsuario,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                new { idUsuario },
                "Roles actualizados correctamente."));
    }

    private async Task<IActionResult> CambiarEstado(
        int idUsuario,
        bool estado,
        CancellationToken cancellationToken)
    {
        if (idUsuario == ObtenerIdUsuarioActual() && !estado)
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    "No puedes desactivar tu propio usuario."));
        }

        var actualizado = await _service.CambiarEstadoAsync(
            idUsuario,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!actualizado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Usuario no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idUsuario, estado },
                estado
                    ? "Usuario activado correctamente."
                    : "Usuario desactivado correctamente."));
    }

    private int ObtenerIdUsuarioActual()
    {
        var valor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(valor, out var idUsuario))
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");
        }

        return idUsuario;
    }
}