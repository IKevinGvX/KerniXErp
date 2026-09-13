using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Repositories.IA;

public sealed class MovimientoAnomaloRepository
    : IMovimientoAnomaloRepository
{
    public async Task<bool> AtenderAnomaliaAsync(
    long idAnomalia,
    int idEmpresa,
    CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_anomalia",
            idAnomalia,
            DbType.Int64);

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        var command = new CommandDefinition(
            "ia.sp_anomalia_atender",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<bool>(
            command);
    }
    private readonly ISqlConnectionFactory _connectionFactory;
    public async Task<AnomaliaDetalleResponse?> ObtenerAnomaliaAsync(
    long idAnomalia,
    int idEmpresa,
    CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_anomalia",
            idAnomalia,
            DbType.Int64);

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        var command = new CommandDefinition(
            "ia.sp_anomalia_obtener",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<AnomaliaDetalleResponse>(
            command);
    }
    public async Task<long> RegistrarAnomaliaAsync(
    int idModeloIA,
    MovimientoAnomaloResponse movimiento,
    CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "@id_modelo_ia",
            idModeloIA,
            DbType.Int32);

        parameters.Add(
            "@id_movimiento_inventario",
            movimiento.IdMovimientoInventario,
            DbType.Int64);

        parameters.Add(
            "@nivel_riesgo",
            movimiento.NivelRiesgo,
            DbType.String);

        parameters.Add(
            "@valor_esperado",
            movimiento.UmbralAnomalia,
            DbType.Decimal);

        parameters.Add(
            "@valor_detectado",
            movimiento.Cantidad,
            DbType.Decimal);

        var descripcion =
            $"Movimiento anómalo detectado para el producto " +
            $"{movimiento.Producto}. " +
            $"Cantidad detectada: {movimiento.Cantidad:N4}. " +
            $"Umbral estadístico: {movimiento.UmbralAnomalia:N4}. " +
            $"Nivel de riesgo: {movimiento.NivelRiesgo}.";

        parameters.Add(
            "@descripcion",
            descripcion,
            DbType.String);

        parameters.Add(
            "@id_anomalia",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        var command =
            new CommandDefinition(
                "ia.sp_anomalia_movimiento_registrar",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<long>(
            "@id_anomalia");
    }
    public async Task<IReadOnlyCollection<AnomaliaRegistradaResponse>>
    ListarAnomaliasAsync(
        int idEmpresa,
        AnomaliaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@estado",
            filtro.Estado,
            DbType.String);

        parameters.Add(
            "@nivel_riesgo",
            filtro.NivelRiesgo,
            DbType.String);

        var command = new CommandDefinition(
            "ia.sp_anomalia_listar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<AnomaliaRegistradaResponse>(
                command);

        return resultado.ToList();
    }
    public MovimientoAnomaloRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<IReadOnlyCollection<MovimientoAnomaloResponse>>
    DetectarAsync(
        int idEmpresa,
        MovimientoAnomaloRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@id_almacen",
            request.IdAlmacen,
            DbType.Int32);

        parameters.Add(
            "@id_producto",
            request.IdProducto,
            DbType.Int32);

        parameters.Add(
            "@dias_historial",
            request.DiasHistorial,
            DbType.Int32);

        // AQUÍ VA
        parameters.Add(
            "@solo_anomalias",
            request.SoloAnomalias,
            DbType.Boolean);

        var command = new CommandDefinition(
            "ia.sp_movimiento_anomalo_detectar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<MovimientoAnomaloResponse>(
                command);

        return resultado.ToList();
    }
}