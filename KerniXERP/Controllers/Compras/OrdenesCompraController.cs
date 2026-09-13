using KernixERP.DTOs.Common;
using KernixERP.DTOs.OrdenesCompra.Requests;
using KernixERP.Interfaces.Compras;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/compras/ordenes")]
public sealed class OrdenesCompraController
    : ControllerBase
{
    private readonly IOrdenCompraService _service;
    private readonly ICurrentUserService _currentUser;

    public OrdenesCompraController(
        IOrdenCompraService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] OrdenCompraFiltroRequest filtro,
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

    [HttpGet("{idOrdenCompra:long}")]
    public async Task<IActionResult> Obtener(
        long idOrdenCompra,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerPorIdAsync(
                idOrdenCompra,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Orden de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    [HttpPost("desde-solicitud")]
    public async Task<IActionResult> GenerarDesdeSolicitud(
        [FromBody] OrdenCompraDesdeSolicitudRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.GenerarDesdeSolicitudAsync(
                request,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idOrdenCompra =
                    resultado.IdOrdenCompra
            },
            ApiResponse<object>.Ok(
                new
                {
                    resultado.IdOrdenCompra,
                    resultado.NumeroOrden
                },
                "Orden de compra generada correctamente."));
    }

    [HttpPost("{idOrdenCompra:long}/anular")]
    public async Task<IActionResult> Anular(
        long idOrdenCompra,
        [FromBody] OrdenCompraAnularRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AnularAsync(
                idOrdenCompra,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Orden de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idOrdenCompra,
                    estado = "anulada"
                },
                "Orden de compra anulada correctamente."));
    }
}