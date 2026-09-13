using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Roles.Responses;
using KernixERP.Interfaces.Seguridad;
using KerniXERP.DTOs.Request.Roles;
using System.Data;

namespace KernixERP.Repositories.Seguridad;

public sealed class RolRepository : IRolRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public RolRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<RolResponse>> ListarAsync(
        RolFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parametros = new
        {
            id_empresa = filtro.IdEmpresa,
            estado = filtro.Estado,
            buscar = filtro.Buscar,
            pagina = filtro.Pagina,
            tamano_pagina = filtro.TamanoPagina
        };

        var command = new CommandDefinition(
            "seg.sp_rol_listar",
            parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var roles = (await multi.ReadAsync<RolResponse>())
            .ToList();

        var total =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<RolResponse>
        {
            Items = roles,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<RolDetalleResponse?> ObtenerPorIdAsync(
        int idRol,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "seg.sp_rol_obtener",
            new { id_rol = idRol },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var rol =
            await multi.ReadSingleOrDefaultAsync<RolDetalleResponse>();

        if (rol is null)
            return null;

        rol.Permisos =
            (await multi.ReadAsync<PermisoRolResponse>())
            .ToList();

        rol.Usuarios =
            (await multi.ReadAsync<UsuarioRolResponse>())
            .ToList();

        return rol;
    }
}