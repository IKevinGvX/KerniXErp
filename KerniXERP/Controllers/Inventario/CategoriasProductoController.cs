using KernixERP.DTOs.CategoriasProducto.Requests;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/categorias-producto")]
public sealed class CategoriasProductoController : ControllerBase
{
    private readonly ICategoriaProductoService _service;

    public CategoriasProductoController(
        ICategoriaProductoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] CategoriaProductoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{idCategoriaProducto:int}")]
    public async Task<IActionResult> Obtener(
        int idCategoriaProducto,
        CancellationToken cancellationToken)
    {
        var result = await _service.ObtenerPorIdAsync(
            idCategoriaProducto,
            cancellationToken);

        if (result is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Categoría no encontrada."));
        }

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] CategoriaProductoCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idCategoriaProducto =
            await _service.CrearAsync(
                request,
                ObtenerIdUsuarioActual(),
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new { idCategoriaProducto },
            ApiResponse<object>.Ok(
                new { idCategoriaProducto },
                "Categoría creada correctamente."));
    }

    [HttpPut("{idCategoriaProducto:int}")]
    public async Task<IActionResult> Actualizar(
        int idCategoriaProducto,
        [FromBody] CategoriaProductoActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ActualizarAsync(
            idCategoriaProducto,
            request,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Categoría no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new { idCategoriaProducto },
                "Categoría actualizada correctamente."));
    }

    [HttpPatch("{idCategoriaProducto:int}/activar")]
    public Task<IActionResult> Activar(
        int idCategoriaProducto,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idCategoriaProducto,
            true,
            cancellationToken);
    }

    [HttpPatch("{idCategoriaProducto:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idCategoriaProducto,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idCategoriaProducto,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idCategoriaProducto,
        bool estado,
        CancellationToken cancellationToken)
    {
        var result = await _service.CambiarEstadoAsync(
            idCategoriaProducto,
            estado,
            ObtenerIdUsuarioActual(),
            cancellationToken);

        if (!result)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Categoría no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idCategoriaProducto,
                    estado
                },
                estado
                    ? "Categoría activada correctamente."
                    : "Categoría desactivada correctamente."));
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