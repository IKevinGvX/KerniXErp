using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.UnidadesMedida.Requests;
using KernixERP.DTOs.UnidadesMedida.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Repositories.Inventario;

public sealed class UnidadMedidaRepository
    : IUnidadMedidaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UnidadMedidaRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<UnidadMedidaResponse>> ListarAsync(
        UnidadMedidaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_unidad_medida_listar",
            parameters: new
            {
                estado = filtro.Estado,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items =
            (await multi.ReadAsync<UnidadMedidaResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<UnidadMedidaResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<UnidadMedidaDetalleResponse?> ObtenerPorIdAsync(
        int idUnidadMedida,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_unidad_medida_obtener",
            parameters: new
            {
                id_unidad_medida = idUnidadMedida
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<UnidadMedidaDetalleResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        UnidadMedidaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@codigo",
            request.Codigo,
            DbType.String);

        parameters.Add(
            "@nombre",
            request.Nombre,
            DbType.String);

        parameters.Add(
            "@abreviatura",
            request.Abreviatura,
            DbType.String);

        parameters.Add(
            "@id_usuario_registro",
            idUsuarioRegistro,
            DbType.Int32);

        parameters.Add(
            "@id_unidad_medida",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_unidad_medida_crear",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return parameters.Get<int>(
            "@id_unidad_medida");
    }

    public async Task<bool> ActualizarAsync(
        int idUnidadMedida,
        UnidadMedidaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_unidad_medida_actualizar",
                parameters: new
                {
                    id_unidad_medida = idUnidadMedida,
                    codigo = request.Codigo,
                    nombre = request.Nombre,
                    abreviatura = request.Abreviatura,
                    estado = request.Estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idUnidadMedida,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_unidad_medida_cambiar_estado",
                parameters: new
                {
                    id_unidad_medida = idUnidadMedida,
                    estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }
}