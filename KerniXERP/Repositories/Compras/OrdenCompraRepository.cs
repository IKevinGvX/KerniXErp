using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.OrdenesCompra.Requests;
using KernixERP.DTOs.OrdenesCompra.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class OrdenCompraRepository
    : IOrdenCompraRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public OrdenCompraRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<OrdenCompraResponse>> ListarAsync(
        OrdenCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_orden_compra_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_sucursal = filtro.IdSucursal,
                id_proveedor = filtro.IdProveedor,
                id_empleado = filtro.IdEmpleado,
                estado = filtro.Estado,
                moneda = filtro.Moneda,
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
            (await multi.ReadAsync<OrdenCompraResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<OrdenCompraResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<OrdenCompraDetalleResponse?> ObtenerPorIdAsync(
        long idOrdenCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_orden_compra_obtener",
            parameters: new
            {
                id_orden_compra = idOrdenCompra,
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var orden =
            await multi.ReadSingleOrDefaultAsync<
                OrdenCompraDetalleResponse>();

        if (orden is null)
        {
            return null;
        }

        orden.Detalles =
            (await multi.ReadAsync<
                DetalleOrdenCompraResponse>())
            .ToList();

        orden.Resumen =
            await multi.ReadSingleOrDefaultAsync<
                OrdenCompraResumenResponse>()
            ?? new OrdenCompraResumenResponse();

        return orden;
    }

    public async Task<(
        long IdOrdenCompra,
        string NumeroOrden)> GenerarDesdeSolicitudAsync(
        OrdenCompraDesdeSolicitudRequest request,
        int idEmpresa,
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_solicitud_compra",
            request.IdSolicitudCompra,
            DbType.Int64);

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@id_proveedor",
            request.IdProveedor,
            DbType.Int32);

        parameters.Add(
            "@id_empleado",
            idEmpleado,
            DbType.Int32);

        parameters.Add(
            "@moneda",
            request.Moneda,
            DbType.String,
            size: 20);

        parameters.Add(
            "@tipo_cambio",
            request.TipoCambio,
            DbType.Decimal);

        parameters.Add(
            "@fecha_entrega_estimada",
            request.FechaEntregaEstimada,
            DbType.Date);

        parameters.Add(
            "@condicion_pago",
            request.CondicionPago,
            DbType.String,
            size: 100);

        parameters.Add(
            "@observacion",
            request.Observacion,
            DbType.String,
            size: 1000);

        parameters.Add(
            "@detalles",
            CrearTablaDetalles(request.Detalles)
                .AsTableValuedParameter(
                    "com.tipo_detalle_orden_desde_solicitud"));

        parameters.Add(
            "@id_orden_compra",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        parameters.Add(
            "@numero_orden",
            dbType: DbType.String,
            size: 40,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText:
                "com.sp_orden_compra_generar_desde_solicitud",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return
        (
            parameters.Get<long>(
                "@id_orden_compra"),

            parameters.Get<string>(
                "@numero_orden")
        );
    }

    public async Task<bool> AnularAsync(
        long idOrdenCompra,
        int idEmpresa,
        int idEmpleado,
        string motivo,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText:
                "com.sp_orden_compra_anular",
            parameters: new
            {
                id_orden_compra = idOrdenCompra,
                id_empresa = idEmpresa,
                id_empleado = idEmpleado,
                motivo
            },
            commandType:
                CommandType.StoredProcedure,
            cancellationToken:
                cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    private static DataTable CrearTablaDetalles(
        IEnumerable<DetalleOrdenDesdeSolicitudRequest>? detalles)
    {
        var table = new DataTable();

        table.Columns.Add(
            "id_detalle_solicitud",
            typeof(long));

        table.Columns.Add(
            "precio_unitario",
            typeof(decimal));

        table.Columns.Add(
            "descuento",
            typeof(decimal));

        table.Columns.Add(
            "porcentaje_igv",
            typeof(decimal));

        foreach (var detalle in detalles ?? [])
        {
            table.Rows.Add(
                detalle.IdDetalleSolicitud,
                detalle.PrecioUnitario,
                detalle.Descuento,
                detalle.PorcentajeIgv);
        }

        return table;
    }
}