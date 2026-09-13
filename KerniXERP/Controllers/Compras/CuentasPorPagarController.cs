using KernixERP.DTOs.Common;
using KernixERP.DTOs.CuentasPorPagar.Requests;
using KernixERP.Interfaces.Compras;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/compras/cuentas-por-pagar")]
public sealed class CuentasPorPagarController
    : ControllerBase
{
    private readonly ICuentaPorPagarService _service;
    private readonly ICurrentUserService _currentUser;

    public CuentasPorPagarController(
        ICuentaPorPagarService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] CuentaPorPagarFiltroRequest filtro,
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

    [HttpGet("{idCuentaPorPagar:long}")]
    public async Task<IActionResult> Obtener(
        long idCuentaPorPagar,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerAsync(
                idCuentaPorPagar,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
            return NotFound(
                ApiResponse<object>.Fail(
                    "Cuenta por pagar no encontrada."));

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    [HttpPost("desde-orden")]
    public async Task<IActionResult> GenerarDesdeOrden(
        [FromBody] CuentaPorPagarDesdeOrdenRequest request,
        CancellationToken cancellationToken)
    {
        var id =
            await _service.GenerarDesdeOrdenAsync(
                request,
                _currentUser.IdEmpresa,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idCuentaPorPagar = id
            },
            ApiResponse<object>.Ok(
                new
                {
                    idCuentaPorPagar = id,
                    request.IdOrdenCompra
                },
                "Cuenta por pagar generada correctamente."));
    }

    [HttpPost("{idCuentaPorPagar:long}/pagos")]
    public async Task<IActionResult> RegistrarPago(
        long idCuentaPorPagar,
        [FromBody] PagoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.RegistrarPagoAsync(
                idCuentaPorPagar,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                request,
                cancellationToken);

        if (!resultado)
            return NotFound(
                ApiResponse<object>.Fail(
                    "Cuenta por pagar no encontrada."));

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idCuentaPorPagar,
                    request.Monto
                },
                "Pago registrado correctamente."));
    }

    [HttpPost("pagos/{idPagoProveedor:long}/revertir")]
    public async Task<IActionResult> RevertirPago(
        long idPagoProveedor,
        [FromBody] PagoProveedorRevertirRequest request,
        CancellationToken cancellationToken)
    {
        await _service.RevertirPagoAsync(
            idPagoProveedor,
            request,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idPagoProveedor
                },
                "Pago revertido correctamente."));
    }
}