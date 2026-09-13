using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.CategoriasProducto.Requests;
using KernixERP.DTOs.CategoriasProducto.Responses;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Repositories.Inventario;

public sealed class CategoriaProductoRepository
    : ICategoriaProductoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CategoriaProductoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<CategoriaProductoResponse>> ListarAsync(
        CategoriaProductoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_categoria_producto_listar",
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

        var items =
            (await multi.ReadAsync<CategoriaProductoResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<CategoriaProductoResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<CategoriaProductoDetalleResponse?> ObtenerPorIdAsync(
        int idCategoriaProducto,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_categoria_producto_obtener",
            parameters: new
            {
                id_categoria_producto = idCategoriaProducto
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<CategoriaProductoDetalleResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        CategoriaProductoCrearRequest request,
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
            "@id_categoria_producto",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_categoria_producto_crear",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return parameters.Get<int>(
            "@id_categoria_producto");
    }

    public async Task<bool> ActualizarAsync(
        int idCategoriaProducto,
        CategoriaProductoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_categoria_producto_actualizar",
                parameters: new
                {
                    id_categoria_producto = idCategoriaProducto,
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
        int idCategoriaProducto,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_categoria_producto_cambiar_estado",
                parameters: new
                {
                    id_categoria_producto = idCategoriaProducto,
                    estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }
}