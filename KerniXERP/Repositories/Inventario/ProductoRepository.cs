using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Productos.Requests;
using KernixERP.DTOs.Productos.Responses;
using KernixERP.Interfaces.Inventario;
using KerniXERP.DTOs.Request.Productos.Response;
using System.Data;

namespace KernixERP.Repositories.Inventario;

public sealed class ProductoRepository : IProductoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ProductoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<ProductoResponse>> ListarAsync(
        ProductoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_categoria_producto = filtro.IdCategoriaProducto,
                id_marca = filtro.IdMarca,
                id_unidad_medida = filtro.IdUnidadMedida,
                id_almacen = filtro.IdAlmacen,
                estado = filtro.Estado,
                controla_stock = filtro.ControlaStock,
                estado_stock = filtro.EstadoStock,
                precio_desde = filtro.PrecioDesde,
                precio_hasta = filtro.PrecioHasta,
                buscar = filtro.Buscar,
                ordenar_por = filtro.OrdenarPor,
                direccion_orden = filtro.DireccionOrden,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var items =
            (await multi.ReadAsync<ProductoResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<ProductoResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<ProductoDetalleResponse?> ObtenerPorIdAsync(
        int idProducto,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_obtener",
            parameters: new
            {
                id_producto = idProducto
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var producto =
            await multi.ReadSingleOrDefaultAsync<
                ProductoDetalleResponse>();

        if (producto is null)
            return null;

        producto.StockPorAlmacen =
            (await multi.ReadAsync<ProductoStockResponse>())
            .ToList();

        producto.Indicadores =
            await multi.ReadSingleOrDefaultAsync<
                ProductoIndicadoresResponse>()
            ?? new ProductoIndicadoresResponse();

        return producto;
    }

    public async Task<ProductoCatalogosResponse> ObtenerCatalogosAsync(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_catalogos",
            parameters: new
            {
                id_empresa = idEmpresa
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        return new ProductoCatalogosResponse
        {
            Categorias =
                (await multi.ReadAsync<CatalogoItemResponse>())
                .ToList(),

            Marcas =
                (await multi.ReadAsync<CatalogoItemResponse>())
                .ToList(),

            UnidadesMedida =
                (await multi.ReadAsync<CatalogoItemResponse>())
                .ToList(),

            Sucursales =
                (await multi.ReadAsync<CatalogoItemResponse>())
                .ToList(),

            Almacenes =
                (await multi.ReadAsync<CatalogoItemResponse>())
                .ToList()
        };
    }

    public async Task<ProductoBusquedaResponse?> BuscarCodigoAsync(
        int idEmpresa,
        string codigo,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_buscar_codigo",
            parameters: new
            {
                id_empresa = idEmpresa,
                codigo
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<ProductoBusquedaResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        ProductoCrearRequest request,
        int idUsuarioRegistro,
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
            "@id_categoria_producto",
            request.IdCategoriaProducto,
            DbType.Int32);

        parameters.Add(
            "@id_marca",
            request.IdMarca,
            DbType.Int32);

        parameters.Add(
            "@id_unidad_medida",
            request.IdUnidadMedida,
            DbType.Int32);

        parameters.Add(
            "@codigo",
            request.Codigo,
            DbType.String);

        parameters.Add(
            "@codigo_barras",
            request.CodigoBarras,
            DbType.String);

        parameters.Add(
            "@nombre",
            request.Nombre,
            DbType.String);

        parameters.Add(
            "@descripcion",
            request.Descripcion,
            DbType.String);

        parameters.Add(
            "@precio_compra",
            request.PrecioCompra,
            DbType.Decimal);

        parameters.Add(
            "@precio_venta",
            request.PrecioVenta,
            DbType.Decimal);

        parameters.Add(
            "@stock_minimo",
            request.StockMinimo,
            DbType.Decimal);

        parameters.Add(
            "@stock_maximo",
            request.StockMaximo,
            DbType.Decimal);

        parameters.Add(
            "@controla_stock",
            request.ControlaStock,
            DbType.Boolean);

        parameters.Add(
            "@id_usuario_registro",
            idUsuarioRegistro,
            DbType.Int32);

        parameters.Add(
            "@id_producto",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_crear",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<int>("@id_producto");
    }

    public async Task<bool> ActualizarAsync(
        int idProducto,
        ProductoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_actualizar",
            parameters: new
            {
                id_producto = idProducto,
                id_empresa = request.IdEmpresa,
                id_categoria_producto = request.IdCategoriaProducto,
                id_marca = request.IdMarca,
                id_unidad_medida = request.IdUnidadMedida,
                codigo = request.Codigo,
                codigo_barras = request.CodigoBarras,
                nombre = request.Nombre,
                descripcion = request.Descripcion,
                precio_compra = request.PrecioCompra,
                precio_venta = request.PrecioVenta,
                stock_minimo = request.StockMinimo,
                stock_maximo = request.StockMaximo,
                controla_stock = request.ControlaStock,
                estado = request.Estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idProducto,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "inv.sp_producto_cambiar_estado",
            parameters: new
            {
                id_producto = idProducto,
                estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }
}