using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.DocumentosProveedor;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class DocumentoProveedorRepository
    : IDocumentoProveedorRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DocumentoProveedorRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<long> RegistrarAsync(
        int idEmpresa,
        DocumentoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@id_empresa", idEmpresa);
        parameters.Add("@id_proveedor", request.IdProveedor);
        parameters.Add(
            "@codigo_tipo_documento",
            request.CodigoTipoDocumento);

        parameters.Add(
            "@id_cuenta_por_pagar",
            request.IdCuentaPorPagar);

        parameters.Add(
            "@id_devolucion_compra",
            request.IdDevolucionCompra);

        parameters.Add(
            "@numero_documento",
            request.NumeroDocumento);

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
            "@total",
            request.Total);

        parameters.Add(
            "@motivo",
            request.Motivo);

        parameters.Add(
            "@observacion",
            request.Observacion);

        parameters.Add(
            "@id_documento_proveedor",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_documento_proveedor_registrar",
                parameters,
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));

        return parameters.Get<long>(
            "@id_documento_proveedor");
    }

    public async Task<PagedResponse<DocumentoProveedorResponse>>
        ListarAsync(
            int idEmpresa,
            DocumentoProveedorFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command =
            new CommandDefinition(
                "com.sp_documento_proveedor_listar",
                new
                {
                    id_empresa = idEmpresa,
                    id_proveedor = filtro.IdProveedor,
                    codigo_tipo = filtro.CodigoTipo,
                    estado = filtro.Estado,
                    fecha_desde = filtro.FechaDesde?.Date,
                    fecha_hasta = filtro.FechaHasta?.Date,
                    buscar = filtro.Buscar,
                    pagina = filtro.Pagina,
                    tamano_pagina = filtro.TamanoPagina
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items =
            (await multi.ReadAsync<
                DocumentoProveedorResponse>())
            .ToList();

        var total =
            await multi.ReadSingleAsync<int>();

        return new PagedResponse<DocumentoProveedorResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<DocumentoProveedorDetalleResponse?>
        ObtenerAsync(
            long idDocumentoProveedor,
            int idEmpresa,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<
                DocumentoProveedorDetalleResponse>(
                new CommandDefinition(
                    "com.sp_documento_proveedor_obtener",
                    new
                    {
                        id_documento_proveedor =
                            idDocumentoProveedor,
                        id_empresa =
                            idEmpresa
                    },
                    commandType:
                        CommandType.StoredProcedure,
                    cancellationToken:
                        cancellationToken));
    }

    public async Task AplicarNotaCreditoAsync(
        long idDocumentoProveedor,
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
                    id_documento_proveedor =
                        idDocumentoProveedor,
                    id_empresa = idEmpresa
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }

    public async Task AplicarNotaDebitoAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_nota_debito_proveedor_aplicar",
                new
                {
                    id_documento_proveedor =
                        idDocumentoProveedor,
                    id_empresa = idEmpresa
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }

    public async Task AnularNotaCreditoAsync(
        long idDocumentoProveedor,
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
                    id_documento_proveedor =
                        idDocumentoProveedor,
                    id_empresa = idEmpresa,
                    observacion
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }

    public async Task AnularNotaDebitoAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        string observacion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "com.sp_nota_debito_proveedor_anular",
                new
                {
                    id_documento_proveedor =
                        idDocumentoProveedor,
                    id_empresa = idEmpresa,
                    observacion
                },
                commandType:
                    CommandType.StoredProcedure,
                cancellationToken:
                    cancellationToken));
    }
}