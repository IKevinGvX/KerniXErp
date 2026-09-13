using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Stock.Requests;
using KernixERP.DTOs.Stock.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Repositories.Inventario;

public sealed class StockRepository : IStockRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public StockRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<StockResponse>> ListarAsync(
        StockFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_stock_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_sucursal = filtro.IdSucursal,
                id_almacen = filtro.IdAlmacen,
                id_categoria_producto =
                    filtro.IdCategoriaProducto,
                id_marca = filtro.IdMarca,
                estado_stock = filtro.EstadoStock,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items =
            (await multi.ReadAsync<StockResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<StockResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<StockDetalleResponse?> ObtenerPorIdAsync(
        int idStock,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_stock_obtener",
            parameters: new
            {
                id_stock = idStock
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<StockDetalleResponse>(
                command);
    }

    public async Task<StockProductoDetalleResponse?> ObtenerPorProductoAsync(
        int idProducto,
        int? idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_stock_producto",
            parameters: new
            {
                id_producto = idProducto,
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var producto =
            await multi.ReadSingleOrDefaultAsync<
                StockProductoCabeceraResponse>();

        if (producto is null)
        {
            return null;
        }

        var stock =
            (await multi.ReadAsync<StockProductoResponse>())
            .ToList();

        var resumen =
            await multi.ReadSingleOrDefaultAsync<
                StockProductoResumenResponse>()
            ?? new StockProductoResumenResponse();

        return new StockProductoDetalleResponse
        {
            Producto = producto,
            StockPorAlmacen = stock,
            Resumen = resumen
        };
    }

    public async Task<bool> ActualizarAsync(
        int idStock,
        StockActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_stock_actualizar",
            parameters: new
            {
                id_stock = idStock,
                stock_minimo = request.StockMinimo,
                stock_maximo = request.StockMaximo,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<StockDashboardResponse> ObtenerDashboardAsync(
        int idEmpresa,
        int? idSucursal,
        int? idAlmacen,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_stock_dashboard",
            parameters: new
            {
                id_empresa = idEmpresa,
                id_sucursal = idSucursal,
                id_almacen = idAlmacen
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var indicadores =
            await multi.ReadSingleOrDefaultAsync<
                StockDashboardIndicadoresResponse>()
            ?? new StockDashboardIndicadoresResponse();

        var stockCritico =
            (await multi.ReadAsync<StockCriticoResponse>())
            .ToList();

        var almacenes =
            (await multi.ReadAsync<
                StockAlmacenResumenResponse>())
            .ToList();

        return new StockDashboardResponse
        {
            Indicadores = indicadores,
            StockCritico = stockCritico,
            Almacenes = almacenes
        };
    }
}