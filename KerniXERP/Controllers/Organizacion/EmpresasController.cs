using System.Security.Claims;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empresas.Requests;
using KernixERP.Interfaces.Organizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Organizacion;

[ApiController]
[Authorize]
[Route("api/v1/empresas")]
public sealed class EmpresasController : ControllerBase
{
    private readonly IEmpresaService _service;

    public EmpresasController(IEmpresaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] EmpresaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(resultado));
    }

    [HttpGet("{idEmpresa:int}")]
    public async Task<IActionResult> Obtener(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idEmpresa,
            cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Empresa no encontrada."));
        }

        return Ok(ApiResponse<object>.Ok(resultado));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] EmpresaCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idEmpresa = await _service.CrearAsync(
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idEmpresa },
            ApiResponse<object>.Ok(
                new { idEmpresa },
                "Empresa creada correctamente."));
    }

    [HttpPut("{idEmpresa:int}")]
    public async Task<IActionResult> Actualizar(
        int idEmpresa,
        [FromBody] EmpresaActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ActualizarAsync(
            idEmpresa,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Empresa no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idEmpresa },
                "Empresa actualizada correctamente."));
    }

    [HttpPatch("{idEmpresa:int}/activar")]
    public Task<IActionResult> Activar(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idEmpresa,
            true,
            cancellationToken);
    }

    [HttpPatch("{idEmpresa:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idEmpresa,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.CambiarEstadoAsync(
            idEmpresa,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Empresa no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idEmpresa, estado },
                estado
                    ? "Empresa activada correctamente."
                    : "Empresa desactivada correctamente."));
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