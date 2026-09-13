using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.NotasCredito;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class NotaCreditoProveedorRepository
    : INotaCreditoProveedorRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public NotaCreditoProveedorRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> RegistrarAsync(
        int idEmpresa,
        NotaCreditoProveedorCrearRequest request,
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
            "@id_proveedor",
            request.IdProveedor);

        parameters.Add(
            "@id_devolucion_compra",
            request.IdDevolucionCompra);

        parameters.Add(
            "@id_cuenta_por_pagar",
            request.IdCuentaPorPagar);

        parameters.Add(
            "@numero_nota",
            request.NumeroNota);

        parameters.Add(
            "@numero_documento_referencia",
            request.NumeroDocumentoReferencia);

        parameters.Add(
            "@fecha_emision",
            request.FechaEmision.Date,
            DbType.Date);

        parameters.Add(
            "@moneda",
            request.Moneda);

        parameters.Add(
            "@monto",
            request.Monto);

        parameters.Add(
            "@motivo",
            request.Motivo);

        parameters.Add(
            "@observacion",
            request.Observacion);

        parameters.Add(
            "@id_nota_credito_proveedor",
            dbType: DbType.Int64,
            direction:
                ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_nota_credito_proveedor_registrar",
                parameters,
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));

        return parameters.Get<long>(
            "@id_nota_credito_proveedor");
    }

    public async Task<
        PagedResponse<NotaCreditoProveedorResponse>>
        ListarAsync(
            int idEmpresa,
            NotaCreditoProveedorFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command =
            new CommandDefinition(
                "com.sp_nota_credito_proveedor_listar",
                new
                {
                    id_empresa =
                        idEmpresa,

                    id_proveedor =
                        filtro.IdProveedor,

                    id_cuenta_por_pagar =
                        filtro.IdCuentaPorPagar,

                    estado =
                        filtro.Estado,

                    fecha_desde =
                        filtro.FechaDesde?.Date,

                    fecha_hasta =
                        filtro.FechaHasta?.Date,

                    buscar =
                        filtro.Buscar,

                    pagina =
                        filtro.Pagina,

                    tamano_pagina =
                        filtro.TamanoPagina
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(
                command);

        var items =
            (await multi.ReadAsync<
                NotaCreditoProveedorResponse>())
            .ToList();

        var total =
            await multi.ReadSingleAsync<int>();

        return new PagedResponse<
            NotaCreditoProveedorResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina =
                filtro.TamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<
        NotaCreditoProveedorDetalleResponse?>
        ObtenerAsync(
            long idNotaCreditoProveedor,
            int idEmpresa,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<
                NotaCreditoProveedorDetalleResponse>(
                new CommandDefinition(
                    "com.sp_nota_credito_proveedor_obtener",
                    new
                    {
                        id_nota_credito_proveedor =
                            idNotaCreditoProveedor,

                        id_empresa =
                            idEmpresa
                    },
                    commandType:
                        CommandType.StoredProcedure,
                    cancellationToken:
                        cancellationToken));
    }

    public async Task AplicarAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_nota_credito_proveedor_aplicar",
                new
                {
                    id_nota_credito_proveedor =
                        idNotaCreditoProveedor,

                    id_empresa =
                        idEmpresa
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }

    public async Task AnularAsync(
        long idNotaCreditoProveedor,
        int idEmpresa,
        string observacion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_nota_credito_proveedor_anular",
                new
                {
                    id_nota_credito_proveedor =
                        idNotaCreditoProveedor,

                    id_empresa =
                        idEmpresa,

                    observacion
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }
}