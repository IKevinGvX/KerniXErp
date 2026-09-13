using KernixERP.DTOs.Common;
using KernixERP.DTOs.SolicitudesCompra.Requests;
using KernixERP.Interfaces.Compras;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Compras;

[ApiController]
[Authorize]
[Route("api/v1/compras/solicitudes")]
public sealed class SolicitudesCompraController
    : ControllerBase
{
    private readonly ISolicitudCompraService _service;
    private readonly ICurrentUserService _currentUser;

    public SolicitudesCompraController(
        ISolicitudCompraService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] SolicitudCompraFiltroRequest filtro,
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

    [HttpGet("{idSolicitudCompra:long}")]
    public async Task<IActionResult> Obtener(
        long idSolicitudCompra,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerPorIdAsync(
                idSolicitudCompra,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Solicitud de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] SolicitudCompraCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdEmpresa =
            _currentUser.IdEmpresa;

        request.IdSucursal =
            _currentUser.IdSucursal;

        var resultado =
            await _service.CrearAsync(
                request,
                _currentUser.IdEmpleado,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idSolicitudCompra =
                    resultado.IdSolicitudCompra
            },
            ApiResponse<object>.Ok(
                new
                {
                    resultado.IdSolicitudCompra,
                    resultado.NumeroSolicitud,
                    idEmpresa =
                        _currentUser.IdEmpresa,
                    idSucursal =
                        _currentUser.IdSucursal,
                    idEmpleadoSolicita =
                        _currentUser.IdEmpleado
                },
                "Solicitud de compra creada correctamente."));
    }

    [HttpPost("{idSolicitudCompra:long}/aprobar")]
    public async Task<IActionResult> Aprobar(
        long idSolicitudCompra,
        [FromBody] SolicitudCompraAprobarRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AprobarAsync(
                idSolicitudCompra,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Solicitud de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idSolicitudCompra,
                    estado = "aprobada"
                },
                "Solicitud de compra aprobada correctamente."));
    }

    [HttpPost("{idSolicitudCompra:long}/rechazar")]
    public async Task<IActionResult> Rechazar(
        long idSolicitudCompra,
        [FromBody] SolicitudCompraProcesarRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.RechazarAsync(
                idSolicitudCompra,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Solicitud de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idSolicitudCompra,
                    estado = "rechazada"
                },
                "Solicitud de compra rechazada correctamente."));
    }

    [HttpPost("{idSolicitudCompra:long}/anular")]
    public async Task<IActionResult> Anular(
        long idSolicitudCompra,
        [FromBody] SolicitudCompraProcesarRequest request,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.AnularAsync(
                idSolicitudCompra,
                _currentUser.IdEmpresa,
                _currentUser.IdEmpleado,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Solicitud de compra no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idSolicitudCompra,
                    estado = "anulada"
                },
                "Solicitud de compra anulada correctamente."));
    }
}