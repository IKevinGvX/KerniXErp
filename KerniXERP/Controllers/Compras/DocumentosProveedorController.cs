using System.Security.Claims;
using KernixERP.DTOs.Compras.DocumentosProveedor;
using KernixERP.Interfaces.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/compras/documentos-proveedor")]
public sealed class DocumentosProveedorController
    : ControllerBase
{
    private readonly IDocumentoProveedorService _service;

    public DocumentosProveedorController(
        IDocumentoProveedorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] DocumentoProveedorFiltroRequest filtro,
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

    [HttpGet("{idDocumentoProveedor:long}")]
    public async Task<IActionResult> Obtener(
        long idDocumentoProveedor,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerAsync(
                idDocumentoProveedor,
                ObtenerIdEmpresa(),
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(new
            {
                success = false,
                message =
                    "Documento de proveedor no encontrado."
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
        [FromBody] DocumentoProveedorCrearRequest request,
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
                idDocumentoProveedor = id
            },
            new
            {
                success = true,
                message =
                    "Documento de proveedor registrado correctamente.",
                data = new
                {
                    idDocumentoProveedor = id,
                    request.CodigoTipoDocumento
                }
            });
    }

    [HttpPost("{idDocumentoProveedor:long}/aplicar")]
    public async Task<IActionResult> Aplicar(
        long idDocumentoProveedor,
        CancellationToken cancellationToken)
    {
        await _service.AplicarAsync(
            idDocumentoProveedor,
            ObtenerIdEmpresa(),
            cancellationToken);

        return Ok(new
        {
            success = true,
            message =
                "Documento aplicado correctamente.",
            data = new
            {
                idDocumentoProveedor
            }
        });
    }

    [HttpPost("{idDocumentoProveedor:long}/anular")]
    public async Task<IActionResult> Anular(
        long idDocumentoProveedor,
        [FromBody] DocumentoProveedorAnularRequest request,
        CancellationToken cancellationToken)
    {
        await _service.AnularAsync(
            idDocumentoProveedor,
            ObtenerIdEmpresa(),
            request,
            cancellationToken);

        return Ok(new
        {
            success = true,
            message =
                "Documento anulado correctamente.",
            data = new
            {
                idDocumentoProveedor
            }
        });
    }

    private int ObtenerIdEmpresa()
    {
        var value =
            User.FindFirstValue("id_empresa");

        if (!int.TryParse(
                value,
                out var idEmpresa)
            ||
            idEmpresa <= 0)
        {
            throw new UnauthorizedAccessException(
                "Empresa no válida en el token.");
        }

        return idEmpresa;
    }
}