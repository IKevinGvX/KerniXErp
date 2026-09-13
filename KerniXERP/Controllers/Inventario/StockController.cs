using KernixERP.DTOs.Common;
using KernixERP.DTOs.Stock.Requests;
using KernixERP.Interfaces.Inventario;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/stock")]
public sealed class StockController : ControllerBase
{
    private readonly IStockService _service;
    private readonly ICurrentUserService _currentUser;

    public StockController(
        IStockService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // ======================================================
    // Listar stock
    // GET /api/v1/stock
    // ======================================================

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] StockFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        /*
         * La empresa siempre se obtiene del JWT.
         * Se ignora cualquier IdEmpresa enviado por el cliente.
         */
        filtro.IdEmpresa = _currentUser.IdEmpresa;

        var resultado = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Obtener stock por ID
    // GET /api/v1/stock/{idStock}
    // ======================================================

    [HttpGet("{idStock:int}")]
    public async Task<IActionResult> Obtener(
        int idStock,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idStock,
            cancellationToken);

        /*
         * Se devuelve 404 tanto si no existe como si pertenece
         * a otra empresa. Así no revelamos información ajena.
         */
        if (resultado is null ||
            resultado.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Registro de stock no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Obtener stock completo de un producto
    // GET /api/v1/stock/producto/{idProducto}
    // ======================================================

    [HttpGet("producto/{idProducto:int}")]
    public async Task<IActionResult> ObtenerPorProducto(
        int idProducto,
        CancellationToken cancellationToken)
    {
        /*
         * Ya no se recibe idEmpresa por query string.
         * Se utiliza directamente la empresa del JWT.
         */
        var resultado =
            await _service.ObtenerPorProductoAsync(
                idProducto,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null ||
            resultado.Producto.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Producto no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Actualizar límites mínimo y máximo
    // PATCH /api/v1/stock/{idStock}
    // ======================================================

    [HttpPatch("{idStock:int}")]
    public async Task<IActionResult> Actualizar(
        int idStock,
        [FromBody] StockActualizarRequest request,
        CancellationToken cancellationToken)
    {
        /*
         * Antes de modificar, comprobamos que el registro
         * pertenezca a la empresa autenticada.
         */
        var stockActual = await _service.ObtenerPorIdAsync(
            idStock,
            cancellationToken);

        if (stockActual is null ||
            stockActual.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Registro de stock no encontrado."));
        }

        var resultado = await _service.ActualizarAsync(
            idStock,
            request,
            _currentUser.IdUsuario,
            cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Registro de stock no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idStock,
                    request.StockMinimo,
                    request.StockMaximo
                },
                "Límites de stock actualizados correctamente."));
    }

    // ======================================================
    // Dashboard de inventario
    // GET /api/v1/stock/dashboard
    // ======================================================

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(
        [FromQuery] int? idSucursal,
        [FromQuery] int? idAlmacen,
        CancellationToken cancellationToken)
    {
        /*
         * La empresa ya no se recibe desde Swagger.
         * Siempre proviene del JWT.
         */
        var resultado =
            await _service.ObtenerDashboardAsync(
                _currentUser.IdEmpresa,
                idSucursal,
                idAlmacen,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }
}