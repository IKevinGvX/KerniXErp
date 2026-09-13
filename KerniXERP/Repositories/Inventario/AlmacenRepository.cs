using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Almacenes.Requests;
using KernixERP.DTOs.Almacenes.Responses;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Repositories.Inventario;

public sealed class AlmacenRepository : IAlmacenRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AlmacenRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<AlmacenResponse>> ListarAsync(
        AlmacenFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_almacen_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_sucursal = filtro.IdSucursal,
                estado = filtro.Estado,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var almacenes = (await multi
            .ReadAsync<AlmacenResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<AlmacenResponse>
        {
            Items = almacenes,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<AlmacenDetalleResponse?> ObtenerPorIdAsync(
        int idAlmacen,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_almacen_obtener",
            parameters: new
            {
                id_almacen = idAlmacen
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<AlmacenDetalleResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        AlmacenCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_sucursal",
            request.IdSucursal,
            DbType.Int32);

        parameters.Add(
            "@codigo",
            request.Codigo,
            DbType.String);

        parameters.Add(
            "@nombre",
            request.Nombre,
            DbType.String);

        parameters.Add(
            "@descripcion",
            request.Descripcion,
            DbType.String);

        parameters.Add(
            "@ubicacion",
            request.Ubicacion,
            DbType.String);

        parameters.Add(
            "@id_usuario_registro",
            idUsuarioRegistro,
            DbType.Int32);

        parameters.Add(
            "@id_almacen",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "inv.sp_almacen_crear",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<int>("@id_almacen");
    }

    public async Task<bool> ActualizarAsync(
        int idAlmacen,
        AlmacenActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_almacen_actualizar",
            parameters: new
            {
                id_almacen = idAlmacen,
                id_sucursal = request.IdSucursal,
                codigo = request.Codigo,
                nombre = request.Nombre,
                descripcion = request.Descripcion,
                ubicacion = request.Ubicacion,
                estado = request.Estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idAlmacen,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_almacen_cambiar_estado",
            parameters: new
            {
                id_almacen = idAlmacen,
                estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }
}