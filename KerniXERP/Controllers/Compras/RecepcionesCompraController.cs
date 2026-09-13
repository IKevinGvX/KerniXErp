using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras;
using KernixERP.Interfaces.Compras;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/compras/recepciones")]
public sealed class RecepcionesCompraController
    : ControllerBase
{
    private readonly IRecepcionCompraService _service;
    private readonly ICurrentUserService _currentUser;

    public RecepcionesCompraController(
        IRecepcionCompraService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] RecepcionCompraFiltroRequest filtro,
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

    [HttpGet("{idRecepcionCompra:long}")]
    public async Task<IActionResult> Obtener(
        long idRecepcionCompra,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerPorIdAsync(
                idRecepcionCompra,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Recepción de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] RecepcionCompraCrearRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.RegistrarAsync(
                request,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idRecepcionCompra =
                    resultado.IdRecepcionCompra
            },
            ApiResponse<object>.Ok(
                new
                {
                    resultado.IdRecepcionCompra,
                    resultado.NumeroRecepcion
                },
                "Recepción de compra registrada correctamente."));
    }

    [HttpPost("{idRecepcionCompra:long}/anular")]
    public async Task<IActionResult> Anular(
        long idRecepcionCompra,
        [FromBody] RecepcionCompraAnularRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AnularAsync(
                idRecepcionCompra,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Recepción de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idRecepcionCompra,
                    estado = "anulada"
                },
                "Recepción de compra anulada correctamente."));
    }
}