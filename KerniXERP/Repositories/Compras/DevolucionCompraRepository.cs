using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.Devoluciones;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class DevolucionCompraRepository
    : IDevolucionCompraRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DevolucionCompraRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DevolucionCompraCrearResponse> RegistrarAsync(
        int idEmpresa,
        int idEmpleado,
        DevolucionCompraCrearRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var tablaDetalles = CrearTablaDetalles(request.Detalles);

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@id_recepcion_compra",
            request.IdRecepcionCompra,
            DbType.Int64);

        parameters.Add(
            "@id_almacen",
            request.IdAlmacen,
            DbType.Int32);

        parameters.Add(
            "@id_empleado",
            idEmpleado,
            DbType.Int32);

        parameters.Add(
            "@motivo",
            request.Motivo,
            DbType.String);

        parameters.Add(
            "@numero_documento_referencia",
            request.NumeroDocumentoReferencia,
            DbType.String);

        parameters.Add(
            "@observacion",
            request.Observacion,
            DbType.String);

        parameters.Add(
            "@detalles",
            tablaDetalles.AsTableValuedParameter(
                "com.tipo_detalle_devolucion_compra"));

        parameters.Add(
            "@id_devolucion_compra",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        parameters.Add(
            "@numero_devolucion",
            dbType: DbType.String,
            size: 30,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            "com.sp_devolucion_compra_registrar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return new DevolucionCompraCrearResponse
        {
            IdDevolucionCompra =
                parameters.Get<long>(
                    "@id_devolucion_compra"),

            NumeroDevolucion =
                parameters.Get<string>(
                    "@numero_devolucion"),

            IdRecepcionCompra =
                request.IdRecepcionCompra
        };
    }

    public async Task<PagedResponse<DevolucionCompraResponse>>
        ListarAsync(
            int idEmpresa,
            DevolucionCompraFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new
        {
            id_empresa = idEmpresa,
            id_proveedor = filtro.IdProveedor,
            id_almacen = filtro.IdAlmacen,
            estado = filtro.Estado,
            fecha_desde = filtro.FechaDesde?.Date,
            fecha_hasta = filtro.FechaHasta?.Date,
            buscar = filtro.Buscar,
            pagina = filtro.Pagina,
            tamano_pagina = filtro.TamanoPagina
        };

        var command = new CommandDefinition(
            "com.sp_devolucion_compra_listar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items =
            (await multi.ReadAsync<DevolucionCompraResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleAsync<int>();

        return new PagedResponse<DevolucionCompraResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<DevolucionCompraDetalleResponse?> ObtenerAsync(
        long idDevolucionCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "com.sp_devolucion_compra_obtener",
            new
            {
                id_devolucion_compra = idDevolucionCompra,
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var cabecera =
            await multi.ReadSingleOrDefaultAsync<
                DevolucionCompraDetalleResponse>();

        if (cabecera is null)
            return null;

        cabecera.Detalles =
            (await multi.ReadAsync<
                DevolucionCompraDetalleItemResponse>())
            .ToList();

        return cabecera;
    }

    public async Task AnularAsync(
        long idDevolucionCompra,
        int idEmpresa,
        int idEmpleado,
        string observacion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "com.sp_devolucion_compra_anular",
            new
            {
                id_devolucion_compra = idDevolucionCompra,
                id_empresa = idEmpresa,
                id_empleado = idEmpleado,
                observacion
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    private static DataTable CrearTablaDetalles(
        IEnumerable<DevolucionCompraDetalleRequest> detalles)
    {
        var table = new DataTable();

        table.Columns.Add(
            "id_detalle_recepcion",
            typeof(long));

        table.Columns.Add(
            "cantidad",
            typeof(decimal));

        table.Columns.Add(
            "motivo",
            typeof(string));

        table.Columns.Add(
            "observacion",
            typeof(string));

        foreach (var detalle in detalles)
        {
            table.Rows.Add(
                detalle.IdDetalleRecepcion,
                detalle.Cantidad,

                string.IsNullOrWhiteSpace(detalle.Motivo)
                    ? DBNull.Value
                    : detalle.Motivo.Trim(),

                string.IsNullOrWhiteSpace(detalle.Observacion)
                    ? DBNull.Value
                    : detalle.Observacion.Trim());
        }

        return table;
    }
}