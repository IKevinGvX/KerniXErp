using KernixERP.DTOs.Clientes.Requests;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Ventas;
using KernixERP.Security.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers.Ventas;

[ApiController]
[Authorize]
[Route("api/v1/clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly IClienteService _service;
    private readonly ICurrentUserService _currentUser;

    public ClientesController(
        IClienteService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ClienteFiltroRequest filtro,
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

    [HttpGet("{idCliente:int}")]
    public async Task<IActionResult> Obtener(
        int idCliente,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.ObtenerPorIdAsync(
                idCliente,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (resultado is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Cliente no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(resultado));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(
        [FromBody] ClienteCrearRequest request,
        CancellationToken cancellationToken)
    {
        request.IdEmpresa =
            _currentUser.IdEmpresa;

        var idCliente =
            await _service.CrearAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(Obtener),
            new
            {
                idCliente
            },
            ApiResponse<object>.Ok(
                new
                {
                    idCliente,
                    idEmpresa =
                        _currentUser.IdEmpresa
                },
                "Cliente creado correctamente."));
    }

    [HttpPut("{idCliente:int}")]
    public async Task<IActionResult> Actualizar(
        int idCliente,
        [FromBody] ClienteActualizarRequest request,
        CancellationToken cancellationToken)
    {
        var clienteActual =
            await _service.ObtenerPorIdAsync(
                idCliente,
                _currentUser.IdEmpresa,
                cancellationToken);

        if (clienteActual is null)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Cliente no encontrado."));
        }

        request.IdEmpresa =
            _currentUser.IdEmpresa;

        var resultado =
            await _service.ActualizarAsync(
                idCliente,
                request,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Cliente no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idCliente
                },
                "Cliente actualizado correctamente."));
    }

    [HttpPatch("{idCliente:int}/activar")]
    public Task<IActionResult> Activar(
        int idCliente,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idCliente,
            true,
            cancellationToken);
    }

    [HttpPatch("{idCliente:int}/desactivar")]
    public Task<IActionResult> Desactivar(
        int idCliente,
        CancellationToken cancellationToken)
    {
        return CambiarEstado(
            idCliente,
            false,
            cancellationToken);
    }

    private async Task<IActionResult> CambiarEstado(
        int idCliente,
        bool estado,
        CancellationToken cancellationToken)
    {
        var resultado =
            await _service.CambiarEstadoAsync(
                idCliente,
                _currentUser.IdEmpresa,
                estado,
                cancellationToken);

        if (!resultado)
        {
            return NotFound(
                ApiResponse<object>.Fail(
                    "Cliente no encontrado."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    idCliente,
                    estado
                },
                estado
                    ? "Cliente activado correctamente."
                    : "Cliente desactivado correctamente."));
    }
}