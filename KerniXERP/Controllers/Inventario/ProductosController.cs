using KernixERP.DTOs.Common;
using KernixERP.DTOs.Productos.Requests;
using KernixERP.Interfaces.Inventario;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/productos")]
public sealed class ProductosController : ControllerBase
{
    private readonly IProductoService _service;
    private readonly ICurrentUserService _currentUser;

    public ProductosController(
        IProductoService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // ======================================================
    // Listar productos
    // GET /api/v1/productos
    // ======================================================

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ProductoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        // La empresa siempre se obtiene desde el JWT.
        filtro.IdEmpresa = _currentUser.IdEmpresa;

        var resultado = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Obtener producto
    // GET /api/v1/productos/{idProducto}
    // ======================================================

    [HttpGet("{idProducto:int}")]
    public async Task<IActionResult> Obtener(
        int idProducto,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idProducto,
            cancellationToken);

        if (resultado is null ||
            resultado.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Catálogos para el formulario de productos
    // GET /api/v1/productos/catalogos
    // ======================================================

    [HttpGet("catalogos")]
    public async Task<IActionResult> ObtenerCatalogos(
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerCatalogosAsync(
                _currentUser.IdEmpresa,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Buscar producto por código interno o código de barras
    // GET /api/v1/productos/buscar-codigo/{codigo}
    // ======================================================

    [HttpGet("buscar-codigo/{codigo}")]
    public async Task<IActionResult> BuscarCodigo(
        string codigo,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.BuscarCodigoAsync(
                _currentUser.IdEmpresa,
                codigo,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Crear producto
    // POST /api/v1/productos
    // ======================================================

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ProductoCrearRequest request,
        CancellationToken cancellationToken)
    {
        /*
         * Se ignora cualquier IdEmpresa enviado por el frontend.
         * El valor real proviene del JWT.
         */
        request.IdEmpresa = _currentUser.IdEmpresa;

        var idProducto =
            await _service.CrearAsync(
                request,
                _currentUser.IdUsuario,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idProducto
            },
            ApiResponse<object>.Ok(
                new
                {
                    idProducto,
                    idEmpresa = _currentUser.IdEmpresa
                },
                "Producto creado correctamente."));
    }

    // ======================================================
    // Actualizar producto
    // PUT /api/v1/productos/{idProducto}
    // ======================================================

    [HttpPut("{idProducto:int}")]
    public async Task<IActionResult> Actualizar(
        int idProducto,
        [FromBody] ProductoActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var productoActual =
            await _service.ObtenerPorIdAsync(
                idProducto,
                cancellationToken);

        if (productoActual is null ||
            productoActual.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        request.IdEmpresa = _currentUser.IdEmpresa;

        var resultado =
            await _service.ActualizarAsync(
                idProducto,
                request,
                _currentUser.IdUsuario,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idProducto,
                    idEmpresa = _currentUser.IdEmpresa
                },
                "Producto actualizado correctamente."));
    }

    // ======================================================
    // Activar producto
    // PATCH /api/v1/productos/{idProducto}/activar
    // ======================================================

    [HttpPatch("{idProducto:int}/activar")]
    public Task<IActionResult> Activar(
        int idProducto,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idProducto,
            true,
            cancellationToken);
    }

    // ======================================================
    // Desactivar producto
    // PATCH /api/v1/productos/{idProducto}/desactivar
    // ======================================================

    [HttpPatch("{idProducto:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idProducto,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idProducto,
            false,
            cancellationToken);
    }

    // ======================================================
    // Método privado para cambiar estado
    // ======================================================

    private async Task<IActionResult> CambiarEstado(
        int idProducto,
        bool estado,
        CancellationToken cancellationToken)
    {
        var productoActual =
            await _service.ObtenerPorIdAsync(
                idProducto,
                cancellationToken);

        if (productoActual is null ||
            productoActual.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        var resultado =
            await _service.CambiarEstadoAsync(
                idProducto,
                estado,
                _currentUser.IdUsuario,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idProducto,
                    estado
                },
                estado
                    ? "Producto activado correctamente."
                    : "Producto desactivado correctamente."));
    }
}