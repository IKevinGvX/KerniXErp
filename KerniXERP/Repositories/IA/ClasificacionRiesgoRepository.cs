// Repositories/IA/ClasificacionRiesgoRepository.cs

using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Repositories.IA;

public sealed class ClasificacionRiesgoRepository
    : IClasificacionRiesgoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ClasificacionRiesgoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ClasificacionRiesgoResponse?>
        ClasificarAsync(
            ClasificacionRiesgoRequest request,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@stock_disponible",
            request.StockDisponible,
            DbType.Decimal);

        parameters.Add(
            "@stock_minimo",
            request.StockMinimo,
            DbType.Decimal);

        parameters.Add(
            "@stock_proyectado",
            request.StockProyectado,
            DbType.Decimal);

        parameters.Add(
            "@nivel_rotacion",
            request.NivelRotacion,
            DbType.String);

        parameters.Add(
            "@es_anomalo",
            request.EsAnomalo,
            DbType.Boolean);

        parameters.Add(
            "@cantidad_movimiento",
            request.CantidadMovimiento,
            DbType.Decimal);

        parameters.Add(
            "@umbral_anomalia",
            request.UmbralAnomalia,
            DbType.Decimal);

        var command =
            new CommandDefinition(
                "ia.sp_riesgo_clasificar",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<ClasificacionRiesgoResponse>(
                command);
    }
}