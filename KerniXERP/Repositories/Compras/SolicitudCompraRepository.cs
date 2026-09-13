using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.SolicitudesCompra.Requests;
using KernixERP.DTOs.SolicitudesCompra.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class SolicitudCompraRepository
    : ISolicitudCompraRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SolicitudCompraRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<SolicitudCompraResponse>> ListarAsync(
        SolicitudCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_solicitud_compra_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_sucursal = filtro.IdSucursal,
                id_empleado_solicita = filtro.IdEmpleadoSolicita,
                id_empleado_aprueba = filtro.IdEmpleadoAprueba,
                prioridad = filtro.Prioridad,
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
            (await multi.ReadAsync<SolicitudCompraResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<SolicitudCompraResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<SolicitudCompraDetalleResponse?> ObtenerPorIdAsync(
        long idSolicitudCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_solicitud_compra_obtener",
            parameters: new
            {
                id_solicitud_compra = idSolicitudCompra,
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var solicitud =
            await multi.ReadSingleOrDefaultAsync<
                SolicitudCompraDetalleResponse>();

        if (solicitud is null)
        {
            return null;
        }

        solicitud.Detalles =
            (await multi.ReadAsync<
                DetalleSolicitudCompraResponse>())
            .ToList();

        solicitud.Resumen =
            await multi.ReadSingleOrDefaultAsync<
                SolicitudCompraResumenResponse>()
            ?? new SolicitudCompraResumenResponse();

        return solicitud;
    }

    public async Task<(long IdSolicitudCompra, string NumeroSolicitud)> CrearAsync(
        SolicitudCompraCrearRequest request,
        int idEmpleadoSolicita,
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
            "@id_sucursal",
            request.IdSucursal,
            DbType.Int32);

        parameters.Add(
            "@id_empleado_solicita",
            idEmpleadoSolicita,
            DbType.Int32);

        parameters.Add(
            "@fecha_necesaria",
            request.FechaNecesaria,
            DbType.Date);

        parameters.Add(
            "@prioridad",
            request.Prioridad,
            DbType.String,
            size: 40);

        parameters.Add(
            "@motivo",
            request.Motivo,
            DbType.String,
            size: 1000);

        parameters.Add(
            "@observacion",
            request.Observacion,
            DbType.String,
            size: 1000);

        parameters.Add(
            "@detalles",
            CrearTablaDetallesCreacion(request.Detalles)
                .AsTableValuedParameter(
                    "com.tipo_detalle_solicitud_compra"));

        parameters.Add(
            "@id_solicitud_compra",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        parameters.Add(
            "@numero_solicitud",
            dbType: DbType.String,
            size: 60,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "com.sp_solicitud_compra_crear",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        var idSolicitudCompra =
            parameters.Get<long>(
                "@id_solicitud_compra");

        var numeroSolicitud =
            parameters.Get<string>(
                "@numero_solicitud");

        return (
            idSolicitudCompra,
            numeroSolicitud);
    }

    public async Task<bool> AprobarAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleadoAprueba,
        SolicitudCompraAprobarRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_solicitud_compra",
            idSolicitudCompra,
            DbType.Int64);

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@id_empleado_aprueba",
            idEmpleadoAprueba,
            DbType.Int32);

        parameters.Add(
            "@observacion",
            request.Observacion,
            DbType.String,
            size: 1000);

        parameters.Add(
            "@detalles",
            CrearTablaDetallesAprobacion(request.Detalles)
                .AsTableValuedParameter(
                    "com.tipo_aprobacion_solicitud_compra"));

        var command = new CommandDefinition(
            commandText: "com.sp_solicitud_compra_aprobar",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> RechazarAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleadoAprueba,
        string observacion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_solicitud_compra_rechazar",
            parameters: new
            {
                id_solicitud_compra = idSolicitudCompra,
                id_empresa = idEmpresa,
                id_empleado_aprueba = idEmpleadoAprueba,
                observacion
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> AnularAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleado,
        string observacion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_solicitud_compra_anular",
            parameters: new
            {
                id_solicitud_compra = idSolicitudCompra,
                id_empresa = idEmpresa,
                id_empleado = idEmpleado,
                observacion
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    private static DataTable CrearTablaDetallesCreacion(
        IEnumerable<DetalleSolicitudCompraCrearRequest>? detalles)
    {
        var table = new DataTable();

        table.Columns.Add(
            "id_producto",
            typeof(int));

        table.Columns.Add(
            "cantidad_solicitada",
            typeof(decimal));

        table.Columns.Add(
            "motivo",
            typeof(string));

        table.Columns.Add(
            "observacion",
            typeof(string));

        foreach (var detalle in detalles ?? [])
        {
            table.Rows.Add(
                detalle.IdProducto,
                detalle.CantidadSolicitada,
                ConvertirDBNull(detalle.Motivo),
                ConvertirDBNull(detalle.Observacion));
        }

        return table;
    }

    private static DataTable CrearTablaDetallesAprobacion(
        IEnumerable<DetalleSolicitudCompraAprobarRequest>? detalles)
    {
        var table = new DataTable();

        table.Columns.Add(
            "id_detalle_solicitud",
            typeof(long));

        table.Columns.Add(
            "cantidad_aprobada",
            typeof(decimal));

        table.Columns.Add(
            "observacion",
            typeof(string));

        foreach (var detalle in detalles ?? [])
        {
            table.Rows.Add(
                detalle.IdDetalleSolicitud,
                detalle.CantidadAprobada,
                ConvertirDBNull(detalle.Observacion));
        }

        return table;
    }

    private static object ConvertirDBNull(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? DBNull.Value
            : value.Trim();
    }
}