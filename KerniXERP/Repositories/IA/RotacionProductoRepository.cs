using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Repositories.IA;

public sealed class RotacionProductoRepository
    : IRotacionProductoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public RotacionProductoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<RotacionProductoResponse>>
        AnalizarAsync(
            int idEmpresa,
            RotacionProductoRequest request,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "@id_empresa",
            idEmpresa,
            DbType.Int32);

        parameters.Add(
            "@id_almacen",
            request.IdAlmacen,
            DbType.Int32);

        parameters.Add(
            "@dias_historial",
            request.DiasHistorial,
            DbType.Int32);

        var command =
            new CommandDefinition(
                "ia.sp_rotacion_productos_analizar",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<RotacionProductoResponse>(
                command);

        return resultado.ToList();
    }
}