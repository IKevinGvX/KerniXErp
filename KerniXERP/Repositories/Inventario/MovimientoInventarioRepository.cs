using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.MovimientosInventario.Requests;
using KernixERP.DTOs.MovimientosInventario.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Repositories.Inventario;

public sealed class MovimientoInventarioRepository
    : IMovimientoInventarioRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public MovimientoInventarioRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<MovimientoInventarioResponse>> ListarAsync(
        MovimientoInventarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_movimiento_inventario_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_sucursal = filtro.IdSucursal,
                id_almacen = filtro.IdAlmacen,
                id_producto = filtro.IdProducto,
                id_tipo_movimiento = filtro.IdTipoMovimiento,
                id_empleado = filtro.IdEmpleado,
                naturaleza = filtro.Naturaleza,
                fecha_desde = filtro.FechaDesde,
                fecha_hasta = filtro.FechaHasta,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items =
            (await multi.ReadAsync<MovimientoInventarioResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<MovimientoInventarioResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<MovimientoInventarioDetalleResponse?> ObtenerPorIdAsync(
        long idMovimientoInventario,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_movimiento_inventario_obtener",
            parameters: new
            {
                id_movimiento_inventario = idMovimientoInventario
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<MovimientoInventarioDetalleResponse>(
                command);
    }

    public async Task<long> RegistrarAsync(
        MovimientoInventarioCrearRequest request,
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_almacen",
            request.IdAlmacen,
            DbType.Int32);

        parameters.Add(
            "@id_producto",
            request.IdProducto,
            DbType.Int32);

        parameters.Add(
            "@id_tipo_movimiento",
            request.IdTipoMovimiento,
            DbType.Int32);

        parameters.Add(
            "@id_empleado",
            idEmpleado,
            DbType.Int32);

        parameters.Add(
            "@cantidad",
            request.Cantidad,
            DbType.Decimal);

        parameters.Add(
            "@referencia",
            request.Referencia,
            DbType.String);

        parameters.Add(
            "@motivo",
            request.Motivo,
            DbType.String);

        parameters.Add(
            "@id_movimiento_inventario",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "inv.sp_movimiento_inventario_registrar",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return parameters.Get<long>(
            "@id_movimiento_inventario");
    }

    public async Task<KardexResponse?> ObtenerKardexAsync(
        KardexFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_kardex_producto",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_producto = filtro.IdProducto,
                id_almacen = filtro.IdAlmacen,
                fecha_desde = filtro.FechaDesde,
                fecha_hasta = filtro.FechaHasta,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var producto =
            await multi.ReadSingleOrDefaultAsync<
                KardexCabeceraResponse>();

        if (producto is null)
            return null;

        var movimientos =
            (await multi.ReadAsync<KardexMovimientoResponse>())
            .ToList();

        var total =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new KardexResponse
        {
            Producto = producto,
            Movimientos =
                new PagedResponse<KardexMovimientoResponse>
                {
                    Items = movimientos,
                    Pagina = filtro.Pagina,
                    TamanoPagina = filtro.TamanoPagina,
                    TotalRegistros = total
                }
        };
    }
}