using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Clientes.Requests;
using KernixERP.DTOs.Clientes.Responses;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Ventas;

namespace KernixERP.Repositories.Ventas;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ClienteRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<ClienteResponse>> ListarAsync(
        ClienteFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ven.sp_cliente_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                tipo_cliente = filtro.TipoCliente,
                tipo_documento = filtro.TipoDocumento,
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
            (await multi.ReadAsync<ClienteResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<ClienteResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<ClienteDetalleResponse?> ObtenerPorIdAsync(
        int idCliente,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ven.sp_cliente_obtener",
            parameters: new
            {
                id_cliente = idCliente,
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var cliente =
            await multi.ReadSingleOrDefaultAsync<
                ClienteDetalleResponse>();

        if (cliente is null)
        {
            return null;
        }

        cliente.Contactos =
            (await multi.ReadAsync<
                ContactoClienteResponse>())
            .ToList();

        cliente.Direcciones =
            (await multi.ReadAsync<
                DireccionClienteResponse>())
            .ToList();

        cliente.Indicadores =
            await multi.ReadSingleOrDefaultAsync<
                ClienteIndicadoresResponse>()
            ?? new ClienteIndicadoresResponse();

        return cliente;
    }

    public async Task<int> CrearAsync(
        ClienteCrearRequest request,
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
            "@tipo_cliente",
            request.TipoCliente,
            DbType.String,
            size: 40);

        parameters.Add(
            "@tipo_documento",
            request.TipoDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@numero_documento",
            request.NumeroDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@razon_social",
            request.RazonSocial,
            DbType.String,
            size: 400);

        parameters.Add(
            "@nombres",
            request.Nombres,
            DbType.String,
            size: 200);

        parameters.Add(
            "@apellidos",
            request.Apellidos,
            DbType.String,
            size: 300);

        parameters.Add(
            "@telefono",
            request.Telefono,
            DbType.String,
            size: 40);

        parameters.Add(
            "@correo",
            request.Correo,
            DbType.String,
            size: 300);

        parameters.Add(
            "@contactos",
            CrearTablaContactos(request.Contactos)
                .AsTableValuedParameter(
                    "ven.tipo_contacto_cliente"));

        parameters.Add(
            "@direcciones",
            CrearTablaDirecciones(request.Direcciones)
                .AsTableValuedParameter(
                    "ven.tipo_direccion_cliente"));

        parameters.Add(
            "@id_cliente",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "ven.sp_cliente_crear",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<int>("@id_cliente");
    }

    public async Task<bool> ActualizarAsync(
        int idCliente,
        ClienteActualizarRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_cliente",
            idCliente,
            DbType.Int32);

        parameters.Add(
            "@id_empresa",
            request.IdEmpresa,
            DbType.Int32);

        parameters.Add(
            "@tipo_cliente",
            request.TipoCliente,
            DbType.String,
            size: 40);

        parameters.Add(
            "@tipo_documento",
            request.TipoDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@numero_documento",
            request.NumeroDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@razon_social",
            request.RazonSocial,
            DbType.String,
            size: 400);

        parameters.Add(
            "@nombres",
            request.Nombres,
            DbType.String,
            size: 200);

        parameters.Add(
            "@apellidos",
            request.Apellidos,
            DbType.String,
            size: 300);

        parameters.Add(
            "@telefono",
            request.Telefono,
            DbType.String,
            size: 40);

        parameters.Add(
            "@correo",
            request.Correo,
            DbType.String,
            size: 300);

        parameters.Add(
            "@estado",
            request.Estado,
            DbType.Boolean);

        parameters.Add(
            "@contactos",
            CrearTablaContactos(request.Contactos)
                .AsTableValuedParameter(
                    "ven.tipo_contacto_cliente"));

        parameters.Add(
            "@direcciones",
            CrearTablaDirecciones(request.Direcciones)
                .AsTableValuedParameter(
                    "ven.tipo_direccion_cliente"));

        var command = new CommandDefinition(
            commandText: "ven.sp_cliente_actualizar",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idCliente,
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ven.sp_cliente_cambiar_estado",
            parameters: new
            {
                id_cliente = idCliente,
                id_empresa = idEmpresa,
                estado
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    private static DataTable CrearTablaContactos(
        IEnumerable<ContactoClienteRequest>? contactos)
    {
        var table = new DataTable();

        table.Columns.Add(
            "nombres",
            typeof(string));

        table.Columns.Add(
            "cargo",
            typeof(string));

        table.Columns.Add(
            "telefono",
            typeof(string));

        table.Columns.Add(
            "correo",
            typeof(string));

        table.Columns.Add(
            "es_principal",
            typeof(bool));

        table.Columns.Add(
            "estado",
            typeof(bool));

        foreach (var contacto in contactos ?? [])
        {
            table.Rows.Add(
                contacto.Nombres,
                ConvertirDBNull(contacto.Cargo),
                ConvertirDBNull(contacto.Telefono),
                ConvertirDBNull(contacto.Correo),
                contacto.EsPrincipal,
                contacto.Estado);
        }

        return table;
    }

    private static DataTable CrearTablaDirecciones(
        IEnumerable<DireccionClienteRequest>? direcciones)
    {
        var table = new DataTable();

        table.Columns.Add(
            "tipo_direccion",
            typeof(string));

        table.Columns.Add(
            "direccion",
            typeof(string));

        table.Columns.Add(
            "referencia",
            typeof(string));

        table.Columns.Add(
            "distrito",
            typeof(string));

        table.Columns.Add(
            "provincia",
            typeof(string));

        table.Columns.Add(
            "departamento",
            typeof(string));

        table.Columns.Add(
            "ubigeo",
            typeof(string));

        table.Columns.Add(
            "es_principal",
            typeof(bool));

        table.Columns.Add(
            "estado",
            typeof(bool));

        foreach (var direccion in direcciones ?? [])
        {
            table.Rows.Add(
                ConvertirDBNull(direccion.TipoDireccion),
                direccion.Direccion,
                ConvertirDBNull(direccion.Referencia),
                ConvertirDBNull(direccion.Distrito),
                ConvertirDBNull(direccion.Provincia),
                ConvertirDBNull(direccion.Departamento),
                ConvertirDBNull(direccion.Ubigeo),
                direccion.EsPrincipal,
                direccion.Estado);
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