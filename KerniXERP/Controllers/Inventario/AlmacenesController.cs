using KernixERP.DTOs.Almacenes.Requests;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Inventario;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Inventario;

[ApiController]
[Authorize]
[Route("api/v1/almacenes")]
public sealed class AlmacenesController : ControllerBase
{
    private readonly IAlmacenService _service;
    private readonly ICurrentUserService _currentUser;

    public AlmacenesController(
        IAlmacenService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // ======================================================
    // Listar almacenes
    // GET /api/v1/almacenes
    // ======================================================

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] AlmacenFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        /*
         * La empresa se obtiene desde el JWT.
         * No se confía en IdEmpresa enviado por el cliente.
         */
        filtro.IdEmpresa = _currentUser.IdEmpresa;

        var resultado = await _service.ListarAsync(
            filtro,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Obtener almacén por ID
    // GET /api/v1/almacenes/{idAlmacen}
    // ======================================================

    [HttpGet("{idAlmacen:int}")]
    public async Task<IActionResult> Obtener(
        int idAlmacen,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idAlmacen,
            cancellationToken);

        if (resultado is null ||
            resultado.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Almacén no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Crear almacén
    // POST /api/v1/almacenes
    // ======================================================

    [HttpPost]
    public async Task<IActionResult> Crear(
     [FromBody] AlmacenCrearRequest request,
     CancellationToken cancellationToken)
    {
        var idAlmacen = await _service.CrearAsync(
            request,
            _currentUser.IdUsuario,
            cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idAlmacen
            },
            ApiResponse<object>.Ok(
                new
                {
                    idAlmacen,
                    idEmpresa = _currentUser.IdEmpresa
                },
                "Almacén creado correctamente."));
    }

    // ======================================================
    // Actualizar almacén
    // PUT /api/v1/almacenes/{idAlmacen}
    // ======================================================
    [HttpPut("{idAlmacen:int}")]
    public async Task<IActionResult> Actualizar(
        int idAlmacen,
        [FromBody] AlmacenActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var almacenActual = await _service.ObtenerPorIdAsync(
            idAlmacen,
            cancellationToken);

        if (almacenActual is null ||
            almacenActual.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Almacén no encontrado."));
        }

        var resultado = await _service.ActualizarAsync(
            idAlmacen,
            request,
            _currentUser.IdUsuario,
            cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Almacén no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idAlmacen,
                    idEmpresa = _currentUser.IdEmpresa
                },
                "Almacén actualizado correctamente."));
    }

    // ======================================================
    // Activar almacén
    // PATCH /api/v1/almacenes/{idAlmacen}/activar
    // ======================================================

    [HttpPatch("{idAlmacen:int}/activar")]
    public Task<IActionResult> Activar(
        int idAlmacen,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idAlmacen,
            true,
            cancellationToken);
    }

    // ======================================================
    // Desactivar almacén
    // PATCH /api/v1/almacenes/{idAlmacen}/desactivar
    // ======================================================

    [HttpPatch("{idAlmacen:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idAlmacen,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idAlmacen,
            false,
            cancellationToken);
    }

    // ======================================================
    // Método privado para cambio de estado
    // ======================================================

    private async Task<IActionResult> CambiarEstado(
        int idAlmacen,
        bool estado,
        CancellationToken cancellationToken)
    {
        var almacenActual = await _service.ObtenerPorIdAsync(
            idAlmacen,
            cancellationToken);

        if (almacenActual is null ||
            almacenActual.IdEmpresa != _currentUser.IdEmpresa)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Almacén no encontrado."));
        }

        var resultado = await _service.CambiarEstadoAsync(
            idAlmacen,
            estado,
            _currentUser.IdUsuario,
            cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Almacén no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idAlmacen,
                    estado
                },
                estado
                    ? "Almacén activado correctamente."
                    : "Almacén desactivado correctamente."));
    }
}