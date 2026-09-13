using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Repositories.IA;

public sealed class AlertaInventarioRepository
    : IAlertaInventarioRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AlertaInventarioRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> GenerarAsync(
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        var command = new CommandDefinition(
            "inv.sp_alertas_stock_generar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<int>(
            command);
    }

    public async Task<IReadOnlyCollection<AlertaInventarioResponse>> ListarAsync(
        int idEmpresa,
        AlertaInventarioFiltroRequest filtro,
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
            filtro.IdAlmacen,
            DbType.Int32);

        parameters.Add(
            "@tipo_alerta",
            filtro.TipoAlerta,
            DbType.String);

        parameters.Add(
            "@estado",
            filtro.Estado,
            DbType.String);

        var command = new CommandDefinition(
            "inv.sp_alerta_stock_listar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<AlertaInventarioResponse>(
                command);

        return resultado.ToList();
    }

    public async Task<AlertaInventarioResponse?> ObtenerPorIdAsync(
        long idAlertaStock,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_alerta_stock",
            idAlertaStock,
            DbType.Int64);

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        var command = new CommandDefinition(
            "inv.sp_alerta_stock_obtener",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<AlertaInventarioResponse>(
                command);
    }

    public async Task AtenderAsync(
        long idAlertaStock,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_alerta_stock",
            idAlertaStock,
            DbType.Int64);

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        var command = new CommandDefinition(
            "inv.sp_alerta_stock_atender",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}