using System.Security.Claims;

namespace KernixERP.Security.CurrentUser;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal Usuario =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException(
            "No existe un contexto HTTP disponible.");

    public bool EstaAutenticado =>
        Usuario.Identity?.IsAuthenticated == true;

    public int IdUsuario =>
        ObtenerClaimEntero(
            ClaimTypes.NameIdentifier,
            "sub");

    public int IdEmpleado =>
        ObtenerClaimEntero("id_empleado");

    public int IdEmpresa =>
        ObtenerClaimEntero("id_empresa");

    public int IdSucursal =>
        ObtenerClaimEntero("id_sucursal");

    public string NombreUsuario =>
        ObtenerClaimTexto(
            ClaimTypes.Name,
            "unique_name",
            "name");

    public string? Correo =>
        ObtenerClaimTextoOpcional(
            ClaimTypes.Email,
            "email");

    public IReadOnlyCollection<string> Roles =>
        Usuario.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Where(value =>
                !string.IsNullOrWhiteSpace(value))
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToArray();

    public IReadOnlyCollection<string> Permisos =>
        Usuario.FindAll("permiso")
            .Select(claim => claim.Value)
            .Where(value =>
                !string.IsNullOrWhiteSpace(value))
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToArray();

    public bool TieneRol(string rol)
    {
        if (string.IsNullOrWhiteSpace(rol))
            return false;

        return Roles.Contains(
            rol.Trim(),
            StringComparer.OrdinalIgnoreCase);
    }

    public bool TienePermiso(string permiso)
    {
        if (string.IsNullOrWhiteSpace(permiso))
            return false;

        return Permisos.Contains(
            permiso.Trim(),
            StringComparer.OrdinalIgnoreCase);
    }

    private int ObtenerClaimEntero(
        params string[] tiposClaim)
    {
        var valor =
            ObtenerClaimTextoOpcional(tiposClaim);

        if (!int.TryParse(valor, out var id) ||
            id <= 0)
        {
            throw new UnauthorizedAccessException(
                $"No se encontró un claim válido: {string.Join(", ", tiposClaim)}.");
        }

        return id;
    }

    private string ObtenerClaimTexto(
        params string[] tiposClaim)
    {
        var valor =
            ObtenerClaimTextoOpcional(tiposClaim);

        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new UnauthorizedAccessException(
                $"No se encontró un claim válido: {string.Join(", ", tiposClaim)}.");
        }

        return valor;
    }

    private string? ObtenerClaimTextoOpcional(
        params string[] tiposClaim)
    {
        foreach (var tipoClaim in tiposClaim)
        {
            var valor =
                Usuario.FindFirstValue(tipoClaim);

            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }
        }

        return null;
    }
}