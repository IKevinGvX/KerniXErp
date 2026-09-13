namespace KernixERP.Security.CurrentUser;

public interface ICurrentUserService
{
    bool EstaAutenticado { get; }

    int IdUsuario { get; }

    int IdEmpleado { get; }

    int IdEmpresa { get; }

    int IdSucursal { get; }

    string NombreUsuario { get; }

    string? Correo { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Permisos { get; }

    bool TieneRol(string rol);

    bool TienePermiso(string permiso);
}