using KernixERP.DTOs.Common;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.IA;

[ApiController]
[Authorize]
[Route("api/v1/ia/alertas-inventario")]
public sealed class AlertasInventarioController : ControllerBase
{
    private readonly IAlertaInventarioService _service;
    private readonly ICurrentUserService _currentUser;

    public AlertasInventarioController(
        IAlertaInventarioService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("debug")]
    public IActionResult Debug()
    {
        return Ok(new
        {
            _currentUser.EstaAutenticado,
            _currentUser.IdUsuario,
            _currentUser.IdEmpleado,
            _currentUser.IdEmpresa,
            _currentUser.IdSucursal,
            _currentUser.NombreUsuario,
            _currentUser.Roles,
            _currentUser.Permisos
        });
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar(
        CancellationToken cancellationToken)
    {
        var cantidad = await _service.GenerarAsync(
            _currentUser.IdEmpresa,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    AlertasGeneradas = cantidad
                },
                "Alertas de inventario generadas correctamente."));
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] AlertaInventarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(
            _currentUser.IdEmpresa,
            filtro,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Alertas de inventario obtenidas correctamente."));
    }

    [HttpPatch("{idAlertaStock:long}/atender")]
    public async Task<IActionResult> Atender(
        long idAlertaStock,
        CancellationToken cancellationToken)
    {
        await _service.AtenderAsync(
            idAlertaStock,
            _currentUser.IdEmpresa,
            cancellationToken);

        return Ok(
            ApiResponse<object>.Ok(
                null,
                "Alerta de inventario atendida correctamente."));

    }
    [HttpGet("{idAlertaStock:long}")]
    public async Task<IActionResult> ObtenerPorId(
    long idAlertaStock,
    CancellationToken cancellationToken)
    {
        var resultado = await _service.ObtenerPorIdAsync(
            idAlertaStock,
            _currentUser.IdEmpresa,
            cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Alerta de inventario no encontrada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                resultado,
                "Alerta de inventario obtenida correctamente."));
    }
}