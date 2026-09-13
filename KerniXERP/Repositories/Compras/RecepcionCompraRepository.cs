using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class RecepcionCompraRepository
    : IRecepcionCompraRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public RecepcionCompraRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<RecepcionCompraResponse>> ListarAsync(
        RecepcionCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_recepcion_compra_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_orden_compra = filtro.IdOrdenCompra,
                id_almacen = filtro.IdAlmacen,
                estado = filtro.Estado,
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
            (await multi.ReadAsync<RecepcionCompraResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<RecepcionCompraResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<RecepcionCompraDetalleResponse?> ObtenerPorIdAsync(
        long idRecepcionCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_recepcion_compra_obtener",
            parameters: new
            {
                id_recepcion_compra = idRecepcionCompra,
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var recepcion =
            await multi.ReadSingleOrDefaultAsync<
                RecepcionCompraDetalleResponse>();

        if (recepcion is null)
        {
            return null;
        }

        recepcion.Detalles =
            (await multi.ReadAsync<
                DetalleRecepcionCompraResponse>())
            .ToList();

        return recepcion;
    }
    public async Task<(
        long IdRecepcionCompra,
        string NumeroRecepcion)> RegistrarAsync(
        RecepcionCompraCrearRequest request,
        int idEmpresa,
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@IdOrdenCompra",
            request.IdOrdenCompra,
            DbType.Int64);

        parameters.Add(
            "@IdAlmacen",
            request.IdAlmacen,
            DbType.Int32);

        parameters.Add(
            "@IdEmpleado",
            idEmpleado,
            DbType.Int32);

        parameters.Add(
            "@IdEmpresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@NumeroGuiaRemision",
            request.NumeroGuiaRemision,
            DbType.String,
            size: 50);

        parameters.Add(
            "@FechaRecepcion",
            request.FechaRecepcion,
            DbType.DateTime2);

        parameters.Add(
            "@TipoRecepcion",
            request.TipoRecepcion,
            DbType.String,
            size: 20);

        parameters.Add(
            "@Observacion",
            request.Observacion,
            DbType.String,
            size: 500);

        parameters.Add(
            "@Detalles",
            CrearTablaDetalles(request.Detalles)
                .AsTableValuedParameter(
                    "com.TVP_DetalleRecepcionCompra"));

        var command = new CommandDefinition(
            commandText:
                "com.usp_recepcion_compra_registrar",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QuerySingleAsync<
                RecepcionCompraCreadaDbResponse>(
                    command);

        return
        (
            resultado.IdRecepcionCompra,
            resultado.NumeroRecepcion
        );
    }

    public async Task<bool> AnularAsync(
      long idRecepcionCompra,
      int idEmpresa,
      int idEmpleado,
      string motivo,
      CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@IdRecepcionCompra",
            idRecepcionCompra,
            DbType.Int64);

        parameters.Add(
            "@IdEmpresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@IdEmpleado",
            idEmpleado,
            DbType.Int32);

        parameters.Add(
            "@Motivo",
            motivo,
            DbType.String,
            size: 500);

        var command = new CommandDefinition(
            commandText:
                "com.usp_recepcion_compra_anular",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QuerySingleAsync<bool>(
                command);

        return resultado;
    }

    private static DataTable CrearTablaDetalles(
        IEnumerable<DetalleRecepcionCompraCrearRequest>? detalles)
    {
        var table =
            new DataTable();

        table.Columns.Add(
            "IdDetalleOrdenCompra",
            typeof(long));

        table.Columns.Add(
            "IdProducto",
            typeof(int));

        table.Columns.Add(
            "CantidadEsperada",
            typeof(decimal));

        table.Columns.Add(
            "CantidadRecibida",
            typeof(decimal));

        table.Columns.Add(
            "CantidadAceptada",
            typeof(decimal));

        table.Columns.Add(
            "CantidadRechazada",
            typeof(decimal));

        table.Columns.Add(
            "CondicionProducto",
            typeof(string));

        table.Columns.Add(
            "Lote",
            typeof(string));

        table.Columns.Add(
            "FechaVencimiento",
            typeof(DateTime));

        table.Columns.Add(
            "Observacion",
            typeof(string));

        foreach (var detalle in detalles ?? [])
        {
            table.Rows.Add(
                detalle.IdDetalleOrdenCompra,
                detalle.IdProducto,
                detalle.CantidadEsperada,
                detalle.CantidadRecibida,
                detalle.CantidadAceptada,
                detalle.CantidadRechazada,
                detalle.CondicionProducto is null
                    ? DBNull.Value
                    : detalle.CondicionProducto,
                detalle.Lote is null
                    ? DBNull.Value
                    : detalle.Lote,
                detalle.FechaVencimiento.HasValue
                    ? detalle.FechaVencimiento.Value
                    : DBNull.Value,
                detalle.Observacion is null
                    ? DBNull.Value
                    : detalle.Observacion);
        }

        return table;
    }

    private sealed class RecepcionCompraCreadaDbResponse
    {
        public long IdRecepcionCompra { get; set; }

        public string NumeroRecepcion { get; set; }
            = string.Empty;
    }
}