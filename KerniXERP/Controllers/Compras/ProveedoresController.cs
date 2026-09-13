using KernixERP.DTOs.Common;
using KernixERP.DTOs.Proveedores.Requests;
using KernixERP.Interfaces.Compras;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/proveedores")]
public sealed class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _service;
    private readonly ICurrentUserService _currentUser;

    public ProveedoresController(
        IProveedorService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    // ======================================================
    // Listar proveedores
    // GET /api/v1/proveedores
    // ======================================================

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ProveedorFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.IdEmpresa =
            _currentUser.IdEmpresa;

        var resultado =
            await _service.ListarAsync(
                filtro,
                cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Obtener proveedor
    // GET /api/v1/proveedores/{idProveedor}
    // ======================================================

    [HttpGet("{idProveedor:int}")]
    public async Task<IActionResult> Obtener(
        int idProveedor,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerPorIdAsync(
                idProveedor,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Proveedor no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    // ======================================================
    // Crear proveedor
    // POST /api/v1/proveedores
    // ======================================================

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdEmpresa =
            _currentUser.IdEmpresa;

        var idProveedor =
            await _service.CrearAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idProveedor
            },
            ApiResponse<object>.Ok(
                new
                {
                    idProveedor,
                    idEmpresa =
                        _currentUser.IdEmpresa
                },
                "Proveedor creado correctamente."));
    }

    // ======================================================
    // Actualizar proveedor
    // PUT /api/v1/proveedores/{idProveedor}
    // ======================================================

    [HttpPut("{idProveedor:int}")]
    public async Task<IActionResult> Actualizar(
        int idProveedor,
        [FromBody] ProveedorActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var proveedorActual =
            await _service.ObtenerPorIdAsync(
                idProveedor,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (proveedorActual is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Proveedor no encontrado."));
        }

        var resultado =
            await _service.ActualizarAsync(
                idProveedor,
                _currentUser.IdEmpresa,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Proveedor no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idProveedor
                },
                "Proveedor actualizado correctamente."));
    }

    // ======================================================
    // Activar proveedor
    // PATCH /api/v1/proveedores/{idProveedor}/activar
    // ======================================================

    [HttpPatch("{idProveedor:int}/activar")]
    public Task<IActionResult> Activar(
        int idProveedor,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idProveedor,
            true,
            cancellationToken);
    }

    // ======================================================
    // Desactivar proveedor
    // PATCH /api/v1/proveedores/{idProveedor}/desactivar
    // ======================================================

    [HttpPatch("{idProveedor:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idProveedor,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idProveedor,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idProveedor,
        bool estado,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.CambiarEstadoAsync(
                idProveedor,
                _currentUser.IdEmpresa,
                estado,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Proveedor no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idProveedor,
                    estado
                },
                estado
                    ? "Proveedor activado correctamente."
                    : "Proveedor desactivado correctamente."));
    }
}