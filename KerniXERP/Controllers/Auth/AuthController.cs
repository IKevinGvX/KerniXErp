using System.Security.Claims;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Requests;
using KernixERP.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var direccionIp =
            HttpContext.Connection.RemoteIpAddress?.ToString();

        var dispositivo =
            Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginAsync(
            request,
            direccionIp,
            dispositivo,
            cancellationToken);

        if (result is null)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "Usuario o contraseña incorrectos, cuenta inactiva o bloqueada."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                result,
                "Inicio de sesión correcto."));
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var result = new
        {
            idUsuario = User.FindFirstValue(
                ClaimTypes.NameIdentifier),

            nombreUsuario = User.FindFirstValue(
                ClaimTypes.Name),

            correo = User.FindFirstValue(
                ClaimTypes.Email),

            idEmpleado = User.FindFirstValue(
                "id_empleado"),

            idEmpresa = User.FindFirstValue(
                "id_empresa"),

            idSucursal = User.FindFirstValue(
                "id_sucursal"),

            roles = User.FindAll(ClaimTypes.Role)
                .Select(x => x.Value),

            permisos = User.FindAll("permiso")
                .Select(x => x.Value)
        };

        return Ok(ApiResponse<object>.Ok(result));
    }
    [AllowAnonymous]
    [HttpGet("generar-hash")]
    public IActionResult GenerarHash()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin123*");

        return Ok(new
        {
            password = "Admin123*",
            hash
        });
    }
    [Authorize]
    [HttpGet("permisos")]
    public async Task<IActionResult> Permisos(
        CancellationToken cancellationToken)
    {
        var idUsuarioTexto = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuarioTexto, out var idUsuario))
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "No se pudo identificar al usuario."));
        }

        var permisos = await _authService.ObtenerPermisosAsync(
            idUsuario,
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(permisos));
    }
}