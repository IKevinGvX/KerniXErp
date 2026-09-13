using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.CuentasPorPagar.Requests;
using KernixERP.DTOs.CuentasPorPagar.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class CuentaPorPagarRepository
    : ICuentaPorPagarRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CuentaPorPagarRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<CuentaPorPagarResponse>>
        ListarAsync(
            CuentaPorPagarFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "com.sp_cuenta_por_pagar_listar",
            new
            {
                id_empresa = filtro.IdEmpresa,
                id_proveedor = filtro.IdProveedor,
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
            (await multi.ReadAsync<CuentaPorPagarResponse>())
            .ToList();

        var total =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<CuentaPorPagarResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<CuentaPorPagarDetalleResponse?> ObtenerAsync(
        long idCuentaPorPagar,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var cuenta =
            await connection.QuerySingleOrDefaultAsync<
                CuentaPorPagarDetalleResponse>(
                new CommandDefinition(
                    "com.sp_cuenta_por_pagar_obtener",
                    new
                    {
                        id_cuenta_por_pagar =
                            idCuentaPorPagar,
                        id_empresa =
                            idEmpresa
                    },
                    commandType:
                        CommandType.StoredProcedure,
                    cancellationToken:
                        cancellationToken));

        if (cuenta is null)
            return null;

        cuenta.Pagos =
            (await ListarPagosAsync(
                idCuentaPorPagar,
                idEmpresa,
                cancellationToken))
            .ToList();

        return cuenta;
    }

    public async Task<long> GenerarDesdeOrdenAsync(
        CuentaPorPagarDesdeOrdenRequest request,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            idEmpresa);

        parameters.Add(
            "@id_orden_compra",
            request.IdOrdenCompra);

        parameters.Add(
            "@numero_documento",
            request.NumeroDocumento);

        parameters.Add(
            "@tipo_documento",
            request.TipoDocumento);

        parameters.Add(
            "@fecha_emision",
            request.FechaEmision,
            DbType.Date);

        parameters.Add(
            "@fecha_vencimiento",
            request.FechaVencimiento,
            DbType.Date);

        parameters.Add(
            "@id_cuenta_por_pagar",
            dbType: DbType.Int64,
            direction:
                ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_cuenta_por_pagar_generar_desde_orden",
                parameters,
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));

        return parameters.Get<long>(
            "@id_cuenta_por_pagar");
    }

    public async Task RegistrarPagoAsync(
        long idCuentaPorPagar,
        int idEmpleado,
        PagoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_pago_proveedor_registrar",
                new
                {
                    id_cuenta_por_pagar =
                        idCuentaPorPagar,
                    id_empleado =
                        idEmpleado,
                    metodo_pago =
                        request.MetodoPago,
                    moneda =
                        request.Moneda,
                    monto =
                        request.Monto,
                    tipo_cambio =
                        request.TipoCambio,
                    numero_operacion =
                        request.NumeroOperacion,
                    observacion =
                        request.Observacion
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }

    public async Task RevertirPagoAsync(
        long idPagoProveedor,
        string observacion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_pago_proveedor_revertir",
                new
                {
                    id_pago_proveedor =
                        idPagoProveedor,
                    observacion
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }

    public async Task<IReadOnlyCollection<PagoProveedorResponse>>
        ListarPagosAsync(
            long idCuentaPorPagar,
            int idEmpresa,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var resultado =
            await connection.QueryAsync<PagoProveedorResponse>(
                new CommandDefinition(
                    "com.sp_pago_proveedor_por_cuenta",
                    new
                    {
                        id_cuenta_por_pagar =
                            idCuentaPorPagar,
                        id_empresa =
                            idEmpresa
                    },
                    commandType:
                        CommandType.StoredProcedure,
                    cancellationToken:
                        cancellationToken));

        return resultado.ToList();
    }
}