using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Usuarios.Requests;
using KernixERP.DTOs.Usuarios.Responses;
using KernixERP.Interfaces.Seguridad;

namespace KernixERP.Repositories.Seguridad;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UsuarioRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<UsuarioResponse>> ListarAsync(
        UsuarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add("@id_empresa", filtro.IdEmpresa);
        parametros.Add("@id_sucursal", filtro.IdSucursal);
        parametros.Add("@estado", filtro.Estado);
        parametros.Add("@buscar", filtro.Buscar);
        parametros.Add("@pagina", filtro.Pagina);
        parametros.Add("@tamano_pagina", filtro.TamanoPagina);

        var command = new CommandDefinition(
            "seg.sp_usuario_listar",
            parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await connection.QueryMultipleAsync(command);

        var usuarios = (await multi
            .ReadAsync<UsuarioResponse>())
            .ToList();

        var total = await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<UsuarioResponse>
        {
            Items = usuarios,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<UsuarioDetalleResponse?> ObtenerPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "seg.sp_usuario_obtener",
            new { id_usuario = idUsuario },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await connection.QueryMultipleAsync(command);

        var usuario = await multi
            .ReadSingleOrDefaultAsync<UsuarioDetalleResponse>();

        if (usuario is null)
            return null;

        usuario.Roles = (await multi.ReadAsync<string>()).ToList();
        usuario.Permisos = (await multi.ReadAsync<string>()).ToList();

        return usuario;
    }

    public async Task<int> CrearAsync(
        UsuarioCrearRequest request,
        string claveHash,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add("@id_empleado", request.IdEmpleado);
        parametros.Add("@nombre_usuario", request.NombreUsuario);
        parametros.Add("@correo", request.Correo);
        parametros.Add("@clave_hash", claveHash);
        parametros.Add(
            "@requiere_cambio_clave",
            request.RequiereCambioClave);
        parametros.Add("@id_usuario_registro", idUsuarioRegistro);

        parametros.Add(
            "@id_usuario",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            "seg.sp_usuario_crear",
            parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parametros.Get<int>("@id_usuario");
    }

    public async Task<bool> ActualizarAsync(
    int idUsuario,
    UsuarioActualizarRequest request,
    int idUsuarioModifica,
    CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "seg.sp_usuario_actualizar",
            new
            {
                id_usuario = idUsuario,
                id_empleado = request.IdEmpleado,
                nombre_usuario = request.NombreUsuario,
                correo = request.Correo,
                estado = request.Estado,
                requiere_cambio_clave = request.RequiereCambioClave,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idUsuario,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "seg.sp_usuario_cambiar_estado",
            new
            {
                id_usuario = idUsuario,
                estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }
    public async Task<bool> ActualizarClaveAsync(
    int idUsuario,
    string claveHash,
    bool requiereCambioClave,
    CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "seg.sp_usuario_actualizar_clave",
            new
            {
                id_usuario = idUsuario,
                clave_hash = claveHash,
                requiere_cambio_clave = requiereCambioClave
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }
    public async Task<bool> AsignarRolesAsync(
    int idUsuario,
    IReadOnlyCollection<int> idRoles,
    int idUsuarioRegistro,
    CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    "seg.sp_usuario_roles_limpiar",
                    new
                    {
                        id_usuario = idUsuario
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));

            foreach (var idRol in idRoles.Distinct())
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        "seg.sp_usuario_rol_asignar",
                        new
                        {
                            id_usuario = idUsuario,
                            id_rol = idRol,
                            id_usuario_registro = idUsuarioRegistro
                        },
                        transaction,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
            }

            transaction.Commit();

            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<string?> ObtenerHashAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
            select clave_hash
            from seg.usuario
            where id_usuario = @idUsuario;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<string>(
            new CommandDefinition(
                sql,
                new { idUsuario },
                cancellationToken: cancellationToken));
    }
}
