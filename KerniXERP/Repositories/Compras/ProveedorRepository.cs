using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Proveedores.Requests;
using KernixERP.DTOs.Proveedores.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Repositories.Compras;

public sealed class ProveedorRepository : IProveedorRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ProveedorRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // ======================================================
    // Listar proveedores
    // ======================================================

    public async Task<IReadOnlyCollection<ProveedorResponse>> ListarAsync(
        ProveedorFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_proveedor_listar",
            parameters: new
            {
                IdEmpresa = filtro.IdEmpresa,
                Busqueda = filtro.Busqueda,
                Estado = filtro.Estado
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var proveedores =
            await connection.QueryAsync<ProveedorResponse>(
                command);

        return proveedores.ToList();
    }

    // ======================================================
    // Obtener proveedor con contactos y direcciones
    // ======================================================

    public async Task<ProveedorDetalleResponse?> ObtenerPorIdAsync(
        int idProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_proveedor_obtener",
            parameters: new
            {
                IdProveedor = idProveedor,
                IdEmpresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var proveedor =
            await multi.ReadSingleOrDefaultAsync<
                ProveedorDetalleResponse>();

        if (proveedor is null)
        {
            return null;
        }

        proveedor.Contactos =
            (await multi.ReadAsync<
                ContactoProveedorResponse>())
            .ToList();

        proveedor.Direcciones =
            (await multi.ReadAsync<
                DireccionProveedorResponse>())
            .ToList();

        return proveedor;
    }

    // ======================================================
    // Crear proveedor
    // ======================================================

    public async Task<int> CrearAsync(
        ProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@IdEmpresa",
            request.IdEmpresa,
            DbType.Int32);

        parameters.Add(
            "@TipoDocumento",
            request.TipoDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@NumeroDocumento",
            request.NumeroDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@RazonSocial",
            request.RazonSocial,
            DbType.String,
            size: 400);

        parameters.Add(
            "@NombreComercial",
            request.NombreComercial,
            DbType.String,
            size: 300);

        parameters.Add(
            "@Telefono",
            request.Telefono,
            DbType.String,
            size: 40);

        parameters.Add(
            "@Correo",
            request.Correo,
            DbType.String,
            size: 300);

        parameters.Add(
            "@Contactos",
            CrearTablaContactos(request.Contactos)
                .AsTableValuedParameter(
                    "com.tvp_contacto_proveedor"));

        parameters.Add(
            "@Direcciones",
            CrearTablaDirecciones(request.Direcciones)
                .AsTableValuedParameter(
                    "com.tvp_direccion_proveedor"));

        var command = new CommandDefinition(
            commandText: "com.sp_proveedor_crear",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        /*
         * sp_proveedor_crear termina con:
         *
         * SELECT @IdProveedor AS id_proveedor;
         */
        return await connection.QuerySingleAsync<int>(
            command);
    }

    // ======================================================
    // Actualizar proveedor
    // ======================================================

    public async Task<bool> ActualizarAsync(
        int idProveedor,
        int idEmpresa,
        ProveedorActualizarRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@IdProveedor",
            idProveedor,
            DbType.Int32);

        parameters.Add(
            "@IdEmpresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@TipoDocumento",
            request.TipoDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@NumeroDocumento",
            request.NumeroDocumento,
            DbType.String,
            size: 40);

        parameters.Add(
            "@RazonSocial",
            request.RazonSocial,
            DbType.String,
            size: 400);

        parameters.Add(
            "@NombreComercial",
            request.NombreComercial,
            DbType.String,
            size: 300);

        parameters.Add(
            "@Telefono",
            request.Telefono,
            DbType.String,
            size: 40);

        parameters.Add(
            "@Correo",
            request.Correo,
            DbType.String,
            size: 300);

        parameters.Add(
            "@Contactos",
            CrearTablaContactos(request.Contactos)
                .AsTableValuedParameter(
                    "com.tvp_contacto_proveedor"));

        parameters.Add(
            "@Direcciones",
            CrearTablaDirecciones(request.Direcciones)
                .AsTableValuedParameter(
                    "com.tvp_direccion_proveedor"));

        var command = new CommandDefinition(
            commandText: "com.sp_proveedor_actualizar",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    // ======================================================
    // Cambiar estado
    // ======================================================

    public async Task<bool> CambiarEstadoAsync(
        int idProveedor,
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "com.sp_proveedor_cambiar_estado",
            parameters: new
            {
                IdProveedor = idProveedor,
                IdEmpresa = idEmpresa,
                Estado = estado
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    // ======================================================
    // TVP de contactos
    // Debe coincidir exactamente con:
    // com.tvp_contacto_proveedor
    // ======================================================

    private static DataTable CrearTablaContactos(
        IEnumerable<ContactoProveedorRequest>? contactos)
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

        foreach (var contacto in contactos ?? [])
        {
            table.Rows.Add(
                contacto.Nombres.Trim(),
                ConvertirDBNull(contacto.Cargo),
                ConvertirDBNull(contacto.Telefono),
                ConvertirDBNull(contacto.Correo),
                contacto.EsPrincipal);
        }

        return table;
    }

    // ======================================================
    // TVP de direcciones
    // Debe coincidir exactamente con:
    // com.tvp_direccion_proveedor
    // ======================================================

    private static DataTable CrearTablaDirecciones(
        IEnumerable<DireccionProveedorRequest>? direcciones)
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

        foreach (var direccion in direcciones ?? [])
        {
            table.Rows.Add(
                ConvertirDBNull(
                    direccion.TipoDireccion),

                direccion.Direccion.Trim(),

                ConvertirDBNull(
                    direccion.Referencia),

                ConvertirDBNull(
                    direccion.Distrito),

                ConvertirDBNull(
                    direccion.Provincia),

                ConvertirDBNull(
                    direccion.Departamento),

                ConvertirDBNull(
                    direccion.Ubigeo),

                direccion.EsPrincipal);
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