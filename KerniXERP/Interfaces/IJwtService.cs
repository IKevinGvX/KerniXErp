using KernixERP.Models;

namespace KernixERP.Interfaces;

public interface IJwtService
{
    string GenerarAccessToken(
        UsuarioAuth usuario,
        IEnumerable<string> roles,
        IEnumerable<PermisoUsuario> permisos,
        string jwtId);

    DateTime ObtenerFechaExpiracion();
}