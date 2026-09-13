// Repositories/IA/ResultadoAnalisisRepository.cs

using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Repositories.IA;

public sealed class ResultadoAnalisisRepository
    : IResultadoAnalisisRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ResultadoAnalisisRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ResultadoAnalisisResponse?> RegistrarAsync(
        int idUsuario,
        ResultadoAnalisisRequest request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_modelo_ia",
            request.IdModeloIA,
            DbType.Int32);

        parameters.Add(
            "@id_usuario",
            idUsuario,
            DbType.Int32);

        parameters.Add(
            "@modulo",
            request.Modulo,
            DbType.String);

        parameters.Add(
            "@tipo_analisis",
            request.TipoAnalisis,
            DbType.String);

        parameters.Add(
            "@entidad",
            request.Entidad,
            DbType.String);

        parameters.Add(
            "@id_registro_origen",
            request.IdRegistroOrigen,
            DbType.String);

        parameters.Add(
            "@resultado",
            request.Resultado,
            DbType.String);

        parameters.Add(
            "@nivel_riesgo",
            request.NivelRiesgo,
            DbType.String);

        parameters.Add(
            "@puntaje_riesgo",
            request.PuntajeRiesgo,
            DbType.Int32);

        parameters.Add(
            "@id_resultado_analisis",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        var command =
            new CommandDefinition(
                "ia.sp_resultado_analisis_registrar",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<ResultadoAnalisisResponse>(
                command);
    }

    public async Task<IReadOnlyCollection<ResultadoAnalisisResponse>>
        ListarAsync(
            int idUsuario,
            ResultadoAnalisisFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "@id_usuario",
            idUsuario,
            DbType.Int32);

        parameters.Add(
            "@modulo",
            filtro.Modulo,
            DbType.String);

        parameters.Add(
            "@tipo_analisis",
            filtro.TipoAnalisis,
            DbType.String);

        parameters.Add(
            "@nivel_riesgo",
            filtro.NivelRiesgo,
            DbType.String);

        var command =
            new CommandDefinition(
                "ia.sp_resultado_analisis_listar",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

        var resultado =
            await connection
                .QueryAsync<ResultadoAnalisisResponse>(
                    command);

        return resultado.ToList();
    }
}