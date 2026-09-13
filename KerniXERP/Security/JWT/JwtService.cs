using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KernixERP.Interfaces;
using KernixERP.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KernixERP.Security.JWT;

public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerarAccessToken(
      UsuarioAuth usuario,
      IEnumerable<string> roles,
      IEnumerable<PermisoUsuario> permisos,
      string jwtId)
    {
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
        new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
        new(ClaimTypes.Name, usuario.NombreUsuario),
        new(JwtRegisteredClaimNames.Jti, jwtId)
    };

        if (!string.IsNullOrWhiteSpace(usuario.Correo))
        {
            claims.Add(new Claim(
                JwtRegisteredClaimNames.Email,
                usuario.Correo));
        }

        if (usuario.IdEmpleado.HasValue)
            claims.Add(new Claim("id_empleado", usuario.IdEmpleado.Value.ToString()));

        if (usuario.IdEmpresa.HasValue)
            claims.Add(new Claim("id_empresa", usuario.IdEmpresa.Value.ToString()));

        if (usuario.IdSucursal.HasValue)
            claims.Add(new Claim("id_sucursal", usuario.IdSucursal.Value.ToString()));

        foreach (var rol in roles.Distinct())
            claims.Add(new Claim(ClaimTypes.Role, rol));

        foreach (var permiso in permisos.Select(x => x.Codigo).Distinct())
            claims.Add(new Claim("permiso", permiso));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: ObtenerFechaExpiracion(),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime ObtenerFechaExpiracion()
    {
        return DateTime.UtcNow.AddMinutes(
            _settings.AccessTokenMinutes);
    }
}