using System.Security.Claims;
using KernixERP.DTOs.Compras.NotasCredito;
using KernixERP.Interfaces.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route(
    "api/v1/compras/notas-credito-proveedor")]
public sealed class NotasCreditoProveedorController
    : ControllerBase
{
    private readonly INotaCreditoProveedorService _service;

    public NotasCreditoProveedorController(
        INotaCreditoProveedorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery]
        NotaCreditoProveedorFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ListarAsync(
                ObtenerIdEmpresa(),
                filtro,
                cancellationToken);

        return Ok(new
        {
            success = true,
            message =
                "Operación realizada correctamente.",
            data = resultado
        });
    }

    [HttpGet("{idNotaCreditoProveedor:long}")]
    public async Task<IActionResult> Obtener(
        long idNotaCreditoProveedor,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerAsync(
                idNotaCreditoProveedor,
                ObtenerIdEmpresa(),
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(new
            {
                success = false,
                message =
                    "Nota de crédito no encontrada."
            });
        }

        return Ok(new
        {
            success = true,
            message =
                "Operación realizada correctamente.",
            data = resultado
        });
    }

    [HttpPost]
    public async Task<IActionResult> Registrar(
        [FromBody]
        NotaCreditoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        var id =
            await _service.RegistrarAsync(
                ObtenerIdEmpresa(),
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idNotaCreditoProveedor = id
            },
            new
            {
                success = true,
                message =
                    "Nota de crédito registrada correctamente.",
                data = new
                {
                    idNotaCreditoProveedor = id
                }
            });
    }

    [HttpPost(
        "{idNotaCreditoProveedor:long}/aplicar")]
    public async Task<IActionResult> Aplicar(
        long idNotaCreditoProveedor,
        CancellationToken cancellationToken)
    {
        await _service.AplicarAsync(
            idNotaCreditoProveedor,
            ObtenerIdEmpresa(),
            cancellationToken);

        return Ok(new
        {
            success = true,
            message =
                "Nota de crédito aplicada correctamente.",
            data = new
            {
                idNotaCreditoProveedor
            }
        });
    }

    [HttpPost(
        "{idNotaCreditoProveedor:long}/anular")]
    public async Task<IActionResult> Anular(
        long idNotaCreditoProveedor,
        [FromBody]
        NotaCreditoProveedorAnularRequest request,
        CancellationToken cancellationToken)
    {
        await _service.AnularAsync(
            idNotaCreditoProveedor,
            ObtenerIdEmpresa(),
            request,
            cancellationToken);

        return Ok(new
        {
            success = true,
            message =
                "Nota de crédito anulada correctamente.",
            data = new
            {
                idNotaCreditoProveedor
            }
        });
    }

    private int ObtenerIdEmpresa()
    {
        var valor =
            User.FindFirstValue(
                "id_empresa");

        if (!int.TryParse(
                valor,
                out var idEmpresa)
            ||
            idEmpresa <= 0)
        {
            throw new
                UnauthorizedAccessException(
                    "Empresa no válida en el token.");
        }

        return idEmpresa;
    }
}