using KernixERP.DTOs.Common;
using KernixERP.DTOs.MovimientosInventario.Requests;
using KernixERP.Interfaces.Inventario;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/inventario")]
public sealed class MovimientosInventarioController : ControllerBase
{
    private readonly IMovimientoInventarioService _service;
    private readonly ICurrentUserService _currentUser;

    public MovimientosInventarioController(
        IMovimientoInventarioService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // ======================================================
    // Listar movimientos
    // GET /api/v1/inventario/movimientos
    // ======================================================

    [HttpGet("movimientos")]
    public async Task<IActionResult> Listar(
        [FromQuery] MovimientoInventarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        /*
         * La empresa siempre se toma del JWT.
         * No se confía en un IdEmpresa enviado por el cliente.
         */
        filtro.IdEmpresa = _currentUser.IdEmpresa;

        var resultado = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Obtener movimiento
    // GET /api/v1/inventario/movimientos/{id}
    // ======================================================

    [HttpGet("movimientos/{idMovimientoInventario:long}")]
    public async Task<IActionResult> Obtener(
        long idMovimientoInventario,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idMovimientoInventario,
            cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Movimiento de inventario no encontrado."));
        }

        /*
         * Protección multiempresa.
         * Aunque el movimiento exista, no se devuelve si pertenece
         * a una empresa diferente a la del usuario autenticado.
         */
        if (resultado.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Movimiento de inventario no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Registrar movimiento general
    // POST /api/v1/inventario/movimientos
    // ======================================================

    [HttpPost("movimientos")]
    public async Task<IActionResult> Registrar(
        [FromBody] MovimientoInventarioCrearRequest request,
        CancellationToken cancellationToken)
    {
        var idMovimientoInventario =
            await _service.RegistrarAsync(
                request,
                _currentUser.IdEmpleado,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idMovimientoInventario
            },
            ApiResponse<object>.Ok(
                new
                {
                    idMovimientoInventario
                },
                "Movimiento de inventario registrado correctamente."));
    }

    // ======================================================
    // Registrar entrada
    // POST /api/v1/inventario/entradas
    // ======================================================

    [HttpPost("entradas")]
    public Task<IActionResult> RegistrarEntrada(
        [FromBody] MovimientoInventarioCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdTipoMovimiento = 1;

        return RegistrarMovimientoPorTipo(
            request,
            "Entrada de inventario registrada correctamente.",
            cancellationToken);
    }

    // ======================================================
    // Registrar salida
    // POST /api/v1/inventario/salidas
    // ======================================================

    [HttpPost("salidas")]
    public Task<IActionResult> RegistrarSalida(
        [FromBody] MovimientoInventarioCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdTipoMovimiento = 2;

        return RegistrarMovimientoPorTipo(
            request,
            "Salida de inventario registrada correctamente.",
            cancellationToken);
    }

    // ======================================================
    // Registrar ajuste positivo
    // POST /api/v1/inventario/ajustes/positivos
    // ======================================================

    [HttpPost("ajustes/positivos")]
    public Task<IActionResult> RegistrarAjustePositivo(
        [FromBody] MovimientoInventarioCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdTipoMovimiento = 3;

        return RegistrarMovimientoPorTipo(
            request,
            "Ajuste positivo registrado correctamente.",
            cancellationToken);
    }

    // ======================================================
    // Registrar ajuste negativo
    // POST /api/v1/inventario/ajustes/negativos
    // ======================================================

    [HttpPost("ajustes/negativos")]
    public Task<IActionResult> RegistrarAjusteNegativo(
        [FromBody] MovimientoInventarioCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdTipoMovimiento = 4;

        return RegistrarMovimientoPorTipo(
            request,
            "Ajuste negativo registrado correctamente.",
            cancellationToken);
    }

    // ======================================================
    // Consultar Kardex
    // GET /api/v1/inventario/kardex
    // ======================================================

    [HttpGet("kardex")]
    public async Task<IActionResult> Kardex(
        [FromQuery] KardexFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        /*
         * La empresa se fuerza desde el JWT.
         */
        filtro.IdEmpresa = _currentUser.IdEmpresa;

        var resultado =
            await _service.ObtenerKardexAsync(
                filtro,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "No se encontró el producto solicitado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Kardex por producto
    // GET /api/v1/inventario/kardex/producto/{idProducto}
    // ======================================================

    [HttpGet("kardex/producto/{idProducto:int}")]
    public async Task<IActionResult> KardexProducto(
        int idProducto,
        [FromQuery] int? idAlmacen,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 50,
        CancellationToken cancellationToken = default)
    {
        var filtro = new KardexFiltroRequest
        {
            IdEmpresa = _currentUser.IdEmpresa,
            IdProducto = idProducto,
            IdAlmacen = idAlmacen,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Pagina = pagina,
            TamanoPagina = tamanoPagina
        };

        var resultado =
            await _service.ObtenerKardexAsync(
                filtro,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "No se encontró el producto solicitado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Método privado para entradas, salidas y ajustes
    // ======================================================

    private async Task<IActionResult> RegistrarMovimientoPorTipo(
        MovimientoInventarioCrearRequest request,
        string mensaje,
        CancellationToken cancellationToken)
    {
        var idMovimientoInventario =
            await _service.RegistrarAsync(
                request,
                _currentUser.IdEmpleado,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idMovimientoInventario
            },
            ApiResponse<object>.Ok(
                new
                {
                    idMovimientoInventario,
                    request.IdAlmacen,
                    request.IdProducto,
                    request.IdTipoMovimiento,
                    request.Cantidad,
                    idEmpleado = _currentUser.IdEmpleado,
                    idEmpresa = _currentUser.IdEmpresa
                },
                mensaje));
    }
}