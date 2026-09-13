using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empresas.Requests;
using KernixERP.DTOs.Empresas.Responses;
using KernixERP.Interfaces.Organizacion;

namespace KernixERP.Repositories.Organizacion;

public sealed class EmpresaRepository : IEmpresaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public EmpresaRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<EmpresaResponse>> ListarAsync(
        EmpresaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add(
            "@estado",
            filtro.Estado,
            DbType.Boolean);

        parametros.Add(
            "@buscar",
            filtro.Buscar,
            DbType.String);

        parametros.Add(
            "@pagina",
            filtro.Pagina,
            DbType.Int32);

        parametros.Add(
            "@tamano_pagina",
            filtro.TamanoPagina,
            DbType.Int32);

        var command = new CommandDefinition(
            commandText: "org.sp_empresa_listar",
            parameters: parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var empresas =
            (await multi.ReadAsync<EmpresaResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<EmpresaResponse>
        {
            Items = empresas,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<EmpresaDetalleResponse?> ObtenerPorIdAsync(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        var command = new CommandDefinition(
            commandText: "org.sp_empresa_obtener",
            parameters: parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<EmpresaDetalleResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        EmpresaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add(
            "@razon_social",
            request.RazonSocial,
            DbType.String);

        parametros.Add(
            "@nombre_comercial",
            request.NombreComercial,
            DbType.String);

        parametros.Add(
            "@ruc",
            request.Ruc,
            DbType.String);

        parametros.Add(
            "@direccion_fiscal",
            request.DireccionFiscal,
            DbType.String);

        parametros.Add(
            "@telefono",
            request.Telefono,
            DbType.String);

        parametros.Add(
            "@correo",
            request.Correo,
            DbType.String);

        parametros.Add(
            "@sitio_web",
            request.SitioWeb,
            DbType.String);

        parametros.Add(
            "@logo_url",
            request.LogoUrl,
            DbType.String);

        parametros.Add(
            "@id_usuario_registro",
            idUsuarioRegistro,
            DbType.Int32);

        parametros.Add(
            "@id_empresa",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "org.sp_empresa_crear",
            parameters: parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parametros.Get<int>("@id_empresa");
    }

    public async Task<bool> ActualizarAsync(
        int idEmpresa,
        EmpresaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parametros.Add(
            "@razon_social",
            request.RazonSocial,
            DbType.String);

        parametros.Add(
            "@nombre_comercial",
            request.NombreComercial,
            DbType.String);

        parametros.Add(
            "@ruc",
            request.Ruc,
            DbType.String);

        parametros.Add(
            "@direccion_fiscal",
            request.DireccionFiscal,
            DbType.String);

        parametros.Add(
            "@telefono",
            request.Telefono,
            DbType.String);

        parametros.Add(
            "@correo",
            request.Correo,
            DbType.String);

        parametros.Add(
            "@sitio_web",
            request.SitioWeb,
            DbType.String);

        parametros.Add(
            "@logo_url",
            request.LogoUrl,
            DbType.String);

        parametros.Add(
            "@estado",
            request.Estado,
            DbType.Boolean);

        parametros.Add(
            "@id_usuario_modifica",
            idUsuarioModifica,
            DbType.Int32);

        var command = new CommandDefinition(
            commandText: "org.sp_empresa_actualizar",
            parameters: parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idEmpresa,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parametros = new DynamicParameters();

        parametros.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parametros.Add(
            "@estado",
            estado,
            DbType.Boolean);

        parametros.Add(
            "@id_usuario_modifica",
            idUsuarioModifica,
            DbType.Int32);

        var command = new CommandDefinition(
            commandText: "org.sp_empresa_cambiar_estado",
            parameters: parametros,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }
}