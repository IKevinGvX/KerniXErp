using System.Security.Claims;
using KernixERP.DTOs.Compras.Devoluciones;
using KernixERP.Interfaces.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/compras/devoluciones")]
public sealed class DevolucionesCompraController
    : ControllerBase
{
    private readonly IDevolucionCompraService _service;

    public DevolucionesCompraController(
        IDevolucionCompraService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] DevolucionCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ListarAsync(
                ObtenerIdEmpresa(),
                filtro,
                cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("{idDevolucionCompra:long}")]
    public async Task<IActionResult> Obtener(
        long idDevolucionCompra,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerAsync(
                idDevolucionCompra,
                ObtenerIdEmpresa(),
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Devolución de compra no encontrada."
            });
        }

        return Ok(new
        {
            success = true,
            message = "Operación realizada correctamente.",
            data = resultado
        });
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody] DevolucionCompraCrearRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.RegistrarAsync(
                ObtenerIdEmpresa(),
                ObtenerIdEmpleado(),
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idDevolucionCompra =
                    resultado.IdDevolucionCompra
            },
            new
            {
                success = true,
                message =
                    "Devolución a proveedor registrada correctamente.",
                data = resultado
            });
    }

    [HttpPost("{idDevolucionCompra:long}/anular")]
    public async Task<IActionResult> Anular(
        long idDevolucionCompra,
        [FromBody] DevolucionCompraAnularRequest request,
        CancellationToken cancellationToken)
    {
        await _service.AnularAsync(
            idDevolucionCompra,
            ObtenerIdEmpresa(),
            ObtenerIdEmpleado(),
            request,
            cancellationToken);

        return Ok(new
        {
            success = true,
            message =
                "Devolución de compra anulada correctamente.",
            data = new
            {
                idDevolucionCompra
            }
        });
    }

    private int ObtenerIdEmpresa()
    {
        var valor =
            User.FindFirstValue("id_empresa");

        if (!int.TryParse(valor, out var id) ||
            id <= 0)
        {
            throw new UnauthorizedAccessException(
                "El token no contiene una empresa válida.");
        }

        return id;
    }

    private int ObtenerIdEmpleado()
    {
        var valor =
            User.FindFirstValue("id_empleado");

        if (!int.TryParse(valor, out var id) ||
            id <= 0)
        {
            throw new UnauthorizedAccessException(
                "El token no contiene un empleado válido.");
        }

        return id;
    }
}