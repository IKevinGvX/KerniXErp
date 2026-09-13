using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.Interfaces;
using KernixERP.Models;

namespace KernixERP.Repositories;

public sealed class AuthRepository : IAuthRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AuthRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UsuarioAuth?> ObtenerUsuarioAsync(
        string usuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
            select top 1
                u.id_usuario                as IdUsuario,
                u.id_empleado               as IdEmpleado,
                e.id_empresa                as IdEmpresa,
                e.id_sucursal               as IdSucursal,
                u.nombre_usuario            as NombreUsuario,
                u.correo                    as Correo,
                u.clave_hash                as ClaveHash,
                concat(
                    e.nombres,
                    ' ',
                    e.apellido_paterno,
                    case
                        when e.apellido_materno is null then ''
                        else concat(' ', e.apellido_materno)
                    end
                )                           as NombreCompleto,
                emp.razon_social            as Empresa,
                s.nombre                    as Sucursal,
                u.requiere_cambio_clave     as RequiereCambioClave,
                u.estado                    as Estado,
                u.intentos_fallidos         as IntentosFallidos,
                u.bloqueado_hasta           as BloqueadoHasta
            from seg.usuario u
            left join org.empleado e
                on e.id_empleado = u.id_empleado
            left join org.empresa emp
                on emp.id_empresa = e.id_empresa
            left join org.sucursal s
                on s.id_sucursal = e.id_sucursal
            where u.nombre_usuario = @usuario
               or u.correo = @usuario;
            """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { usuario },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<UsuarioAuth>(command);
    }

    public async Task<IReadOnlyCollection<string>> ObtenerRolesAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
            select
                r.nombre
            from seg.usuario_rol ur
            inner join seg.rol r
                on r.id_rol = ur.id_rol
            where ur.id_usuario = @idUsuario
              and ur.estado = 1
              and r.estado = 1;
            """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { idUsuario },
            cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<string>(command);

        return result.ToList();
    }

    public async Task<IReadOnlyCollection<PermisoUsuario>> ObtenerPermisosAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
            select
                id_permiso as IdPermiso,
                codigo     as Codigo,
                nombre     as Nombre,
                modulo     as Modulo,
                accion     as Accion,
                id_rol     as IdRol,
                rol        as Rol
            from seg.fn_permisos_usuario(@idUsuario);
            """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { idUsuario },
            cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<PermisoUsuario>(command);

        return result.ToList();
    }
    public async Task GuardarRefreshTokenAsync(
    RefreshTokenModel refreshToken,
    CancellationToken cancellationToken)
    {
        const string sql = """
        insert into seg.refresh_token
        (
            id_usuario,
            token_hash,
            jwt_id,
            fecha_creacion,
            fecha_expiracion,
            direccion_ip,
            dispositivo
        )
        values
        (
            @IdUsuario,
            @TokenHash,
            @JwtId,
            @FechaCreacion,
            @FechaExpiracion,
            @DireccionIp,
            @Dispositivo
        );
        """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            refreshToken,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task<RefreshTokenModel?> ObtenerRefreshTokenAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        const string sql = """
        select top 1
            id_refresh_token       as IdRefreshToken,
            id_usuario             as IdUsuario,
            token_hash             as TokenHash,
            jwt_id                 as JwtId,
            fecha_creacion         as FechaCreacion,
            fecha_expiracion       as FechaExpiracion,
            fecha_revocacion       as FechaRevocacion,
            reemplazado_por_hash   as ReemplazadoPorHash,
            direccion_ip           as DireccionIp,
            dispositivo            as Dispositivo
        from seg.refresh_token
        where token_hash = @tokenHash;
        """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { tokenHash },
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<RefreshTokenModel>(command);
    }

    public async Task RevocarRefreshTokenAsync(
        long idRefreshToken,
        string motivo,
        string? reemplazadoPorHash,
        CancellationToken cancellationToken)
    {
        const string sql = """
        update seg.refresh_token
        set fecha_revocacion = sysdatetime(),
            motivo_revocacion = @motivo,
            reemplazado_por_hash = @reemplazadoPorHash
        where id_refresh_token = @idRefreshToken
          and fecha_revocacion is null;
        """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                idRefreshToken,
                motivo,
                reemplazadoPorHash
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task RevocarTokensUsuarioAsync(
        int idUsuario,
        string motivo,
        CancellationToken cancellationToken)
    {
        const string sql = """
        update seg.refresh_token
        set fecha_revocacion = sysdatetime(),
            motivo_revocacion = @motivo
        where id_usuario = @idUsuario
          and fecha_revocacion is null
          and fecha_expiracion > sysdatetime();
        """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { idUsuario, motivo },
                cancellationToken: cancellationToken));
    }

    public async Task<UsuarioAuth?> ObtenerUsuarioPorIdAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
        select top 1
            u.id_usuario            as IdUsuario,
            u.id_empleado           as IdEmpleado,
            e.id_empresa            as IdEmpresa,
            e.id_sucursal           as IdSucursal,
            u.nombre_usuario        as NombreUsuario,
            u.correo                as Correo,
            u.clave_hash            as ClaveHash,
            concat(e.nombres, ' ', e.apellido_paterno)
                                      as NombreCompleto,
            emp.razon_social        as Empresa,
            s.nombre                as Sucursal,
            u.requiere_cambio_clave as RequiereCambioClave,
            u.estado                as Estado,
            u.intentos_fallidos     as IntentosFallidos,
            u.bloqueado_hasta       as BloqueadoHasta
        from seg.usuario u
        left join org.empleado e
            on e.id_empleado = u.id_empleado
        left join org.empresa emp
            on emp.id_empresa = e.id_empresa
        left join org.sucursal s
            on s.id_sucursal = e.id_sucursal
        where u.id_usuario = @idUsuario;
        """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<UsuarioAuth>(
            new CommandDefinition(
                sql,
                new { idUsuario },
                cancellationToken: cancellationToken));
    }
    public async Task RegistrarAccesoAsync(
        int? idUsuario,
        string usuarioIngresado,
        string? direccionIp,
        string? dispositivo,
        string resultado,
        string? motivoFallo,
        CancellationToken cancellationToken)
    {
        const string sql = """
            insert into seg.historial_acceso
            (
                id_usuario,
                nombre_usuario_ingresado,
                direccion_ip,
                dispositivo,
                resultado,
                motivo_fallo,
                fecha_acceso
            )
            values
            (
                @idUsuario,
                @usuarioIngresado,
                @direccionIp,
                @dispositivo,
                @resultado,
                @motivoFallo,
                sysdatetime()
            );
            """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new
            {
                idUsuario,
                usuarioIngresado,
                direccionIp,
                dispositivo,
                resultado,
                motivoFallo
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task RegistrarLoginExitosoAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
            update seg.usuario
            set intentos_fallidos = 0,
                bloqueado_hasta = null,
                ultimo_acceso = sysdatetime(),
                fecha_actualizacion = sysdatetime()
            where id_usuario = @idUsuario;
            """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { idUsuario },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task RegistrarLoginFallidoAsync(
        int idUsuario,
        CancellationToken cancellationToken)
    {
        const string sql = """
            update seg.usuario
            set intentos_fallidos = intentos_fallidos + 1,
                bloqueado_hasta =
                    case
                        when intentos_fallidos + 1 >= 5
                        then dateadd(minute, 15, sysdatetime())
                        else bloqueado_hasta
                    end,
                fecha_actualizacion = sysdatetime()
            where id_usuario = @idUsuario;
            """;

        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { idUsuario },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}