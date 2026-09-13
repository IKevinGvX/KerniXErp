using KernixERP.DTOs.Requests;
using KernixERP.DTOs.Responses;
using KernixERP.Interfaces;
using KernixERP.Models;

namespace KernixERP.Services;

public sealed class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly ITokenService _tokenService;

    public AuthService(
        IAuthRepository authRepository,
        IJwtService jwtService,
        ITokenService tokenService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        string? direccionIp,
        string? dispositivo,
        CancellationToken cancellationToken)
    {
        var usuarioIngresado = request.Usuario.Trim();

        var usuario = await _authRepository.ObtenerUsuarioAsync(
            usuarioIngresado,
            cancellationToken);

        if (usuario is null)
        {
            await _authRepository.RegistrarAccesoAsync(
                null,
                usuarioIngresado,
                direccionIp,
                dispositivo,
                "fallido",
                "usuario no encontrado",
                cancellationToken);

            return null;
        }

        if (!usuario.Estado)
        {
            await RegistrarFalloAsync(
                usuario,
                usuarioIngresado,
                direccionIp,
                dispositivo,
                "usuario inactivo",
                cancellationToken);

            return null;
        }

        if (usuario.BloqueadoHasta.HasValue &&
            usuario.BloqueadoHasta.Value > DateTime.Now)
        {
            await _authRepository.RegistrarAccesoAsync(
                usuario.IdUsuario,
                usuarioIngresado,
                direccionIp,
                dispositivo,
                "fallido",
                "usuario bloqueado temporalmente",
                cancellationToken);

            return null;
        }

        var passwordCorrecto = BCrypt.Net.BCrypt.Verify(
            request.Password,
            usuario.ClaveHash);

        if (!passwordCorrecto)
        {
            await RegistrarFalloAsync(
                usuario,
                usuarioIngresado,
                direccionIp,
                dispositivo,
                "contraseña incorrecta",
                cancellationToken);

            return null;
        }

        var roles = await _authRepository.ObtenerRolesAsync(
            usuario.IdUsuario,
            cancellationToken);

        var permisos = await _authRepository.ObtenerPermisosAsync(
            usuario.IdUsuario,
            cancellationToken);

        await _authRepository.RegistrarLoginExitosoAsync(
            usuario.IdUsuario,
            cancellationToken);

        await _authRepository.RegistrarAccesoAsync(
            usuario.IdUsuario,
            usuarioIngresado,
            direccionIp,
            dispositivo,
            "exitoso",
            null,
            cancellationToken);

        var jwtId = Guid.NewGuid().ToString();

        var accessTokenExpiresAt =
            _jwtService.ObtenerFechaExpiracion();

        var refreshToken =
            _tokenService.GenerarRefreshToken();

        var refreshTokenHash =
            _tokenService.CalcularHash(refreshToken);

        var refreshTokenExpiresAt =
            DateTime.UtcNow.AddDays(7);

        await _authRepository.GuardarRefreshTokenAsync(
            new RefreshTokenModel
            {
                IdUsuario = usuario.IdUsuario,
                TokenHash = refreshTokenHash,
                JwtId = jwtId,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = refreshTokenExpiresAt,
                DireccionIp = direccionIp,
                Dispositivo = dispositivo
            },
            cancellationToken);

        var accessToken = _jwtService.GenerarAccessToken(
            usuario,
            roles,
            permisos,
            jwtId);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,

            Usuario = new UsuarioSesionResponse
            {
                IdUsuario = usuario.IdUsuario,
                IdEmpleado = usuario.IdEmpleado,
                IdEmpresa = usuario.IdEmpresa,
                IdSucursal = usuario.IdSucursal,
                NombreUsuario = usuario.NombreUsuario,
                Correo = usuario.Correo,
                NombreCompleto = usuario.NombreCompleto,
                Empresa = usuario.Empresa,
                Sucursal = usuario.Sucursal,
                RequiereCambioClave = usuario.RequiereCambioClave
            },

            Roles = roles,
            Permisos = permisos
        };
    }

    public Task<IReadOnlyCollection<PermisoUsuario>> ObtenerPermisosAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        return _authRepository.ObtenerPermisosAsync(
            idUsuario,
            cancellationToken);
    }

    private async Task RegistrarFalloAsync(
        UsuarioAuth usuario,
        string usuarioIngresado,
        string? direccionIp,
        string? dispositivo,
        string motivo,
        CancellationToken cancellationToken)
    {
        await _authRepository.RegistrarLoginFallidoAsync(
            usuario.IdUsuario,
            cancellationToken);

        await _authRepository.RegistrarAccesoAsync(
            usuario.IdUsuario,
            usuarioIngresado,
            direccionIp,
            dispositivo,
            "fallido",
            motivo,
            cancellationToken);
    }
}