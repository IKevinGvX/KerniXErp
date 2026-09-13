using KernixERP.DTOs.Common;
using KernixERP.DTOs.Usuarios.Requests;
using KernixERP.DTOs.Usuarios.Responses;
using KernixERP.Interfaces.Seguridad;

namespace KernixERP.Services.Seguridad;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<UsuarioResponse>> ListarAsync(
        UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(filtro.Pagina, 1);
        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        return _repository.ObtenerPorIdAsync(
            idUsuario,
            cancellationToken);
    }

    public async Task<int> CrearAsync(
        UsuarioCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        var claveHash = BCrypt.Net.BCrypt.HashPassword(
            request.Password,
            workFactor: 12);

        var idUsuario = await _repository.CrearAsync(
            request,
            claveHash,
            idUsuarioRegistro,
            cancellationToken);

        if (request.Roles.Count > 0)
        {
            await _repository.AsignarRolesAsync(
                idUsuario,
                request.Roles,
                idUsuarioRegistro,
                cancellationToken);
        }

        return idUsuario;
    }

    public Task<bool> ActualizarAsync(
        int idUsuario,
        UsuarioActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        return _repository.ActualizarAsync(
            idUsuario,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idUsuario,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        return _repository.CambiarEstadoAsync(
            idUsuario,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    public async Task<bool> CambiarPasswordAsync(
     int idUsuario,
     UsuarioCambiarPasswordRequest request,
     CancellationToken cancellationToken)
    {
        var hashActual = await _repository.ObtenerHashAsync(
            idUsuario,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(hashActual))
            return false;

        bool passwordValida;

        try
        {
            passwordValida = BCrypt.Net.BCrypt.Verify(
                request.PasswordActual,
                hashActual);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }

        if (!passwordValida)
            return false;

        if (request.PasswordActual == request.PasswordNueva)
            return false;

        var hashNuevo = BCrypt.Net.BCrypt.HashPassword(
            request.PasswordNueva,
            workFactor: 12);

        return await _repository.ActualizarClaveAsync(
            idUsuario,
            hashNuevo,
            false,
            cancellationToken);
    }

    public Task<bool> AsignarRolesAsync(
        int idUsuario,
        UsuarioRolesRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        return _repository.AsignarRolesAsync(
            idUsuario,
            request.IdRoles,
            idUsuarioRegistro,
            cancellationToken);
    }
}