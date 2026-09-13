using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Marcas.Requests;
using KernixERP.DTOs.Marcas.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Repositories.Inventario;

public sealed class MarcaRepository : IMarcaRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public MarcaRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<MarcaResponse>> ListarAsync(
        MarcaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_marca_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                estado = filtro.Estado,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items = (await multi
            .ReadAsync<MarcaResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<MarcaResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<MarcaDetalleResponse?> ObtenerPorIdAsync(
        int idMarca,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_marca_obtener",
            parameters: new
            {
                id_marca = idMarca
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<MarcaDetalleResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        MarcaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            request.IdEmpresa,
            DbType.Int32);

        parameters.Add(
            "@nombre",
            request.Nombre,
            DbType.String);

        parameters.Add(
            "@descripcion",
            request.Descripcion,
            DbType.String);

        parameters.Add(
            "@id_usuario_registro",
            idUsuarioRegistro,
            DbType.Int32);

        parameters.Add(
            "@id_marca",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_marca_crear",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return parameters.Get<int>("@id_marca");
    }

    public async Task<bool> ActualizarAsync(
        int idMarca,
        MarcaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_marca_actualizar",
                parameters: new
                {
                    id_marca = idMarca,
                    id_empresa = request.IdEmpresa,
                    nombre = request.Nombre,
                    descripcion = request.Descripcion,
                    estado = request.Estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idMarca,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_marca_cambiar_estado",
                parameters: new
                {
                    id_marca = idMarca,
                    estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }
}