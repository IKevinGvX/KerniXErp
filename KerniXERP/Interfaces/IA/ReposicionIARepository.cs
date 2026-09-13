using System.Data;
using System.Text.Json;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Repositories.IA;

public sealed class ReposicionIARepository
    : IReposicionIARepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ReposicionIARepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<RecomendacionReposicionDetalleResponse?>
    ObtenerRecomendacionAsync(
        long idRecomendacionIA,
        int idUsuario,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText:
                "ia.sp_recomendacion_reposicion_obtener",
            parameters: new
            {
                IdRecomendacionIA = idRecomendacionIA,
                IdUsuario = idUsuario
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<
                RecomendacionReposicionDetalleResponse>(
                    command);
    }

    public async Task ProcesarRecomendacionAsync(
        long idRecomendacionIA,
        int idUsuario,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText:
                "ia.sp_recomendacion_reposicion_procesar",
            parameters: new
            {
                IdRecomendacionIA = idRecomendacionIA,
                IdUsuario = idUsuario
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
    public async Task<
        IReadOnlyCollection<RecomendacionReposicionResponse>>
        AnalizarAsync(
            int idEmpresa,
            int idUsuario,
            ReposicionAnalisisRequest request,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ia.sp_reposicion_analizar",
            parameters: new
            {
                id_empresa = idEmpresa,
                id_usuario = idUsuario,
                id_almacen = request.IdAlmacen,
                dias_historial = request.DiasHistorial,
                dias_proyeccion = request.DiasProyeccion
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<
                RecomendacionReposicionResponse>(
                    command);

        return resultado.ToList();
    }

    public async Task<long> GuardarPrediccionAsync(
        int idUsuario,
        RecomendacionReposicionResponse analisis,
        int diasHistorial,
        int diasProyeccion,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters =
            new DynamicParameters();

        var parametrosJson =
            JsonSerializer.Serialize(new
            {
                analisis.IdProducto,
                analisis.CodigoProducto,
                analisis.IdAlmacen,
                analisis.StockActual,
                analisis.StockDisponible,
                analisis.StockMinimo,
                analisis.StockMaximo,
                analisis.ConsumoPeriodo,
                analisis.ConsumoPromedioDiario,
                analisis.DemandaProyectada,
                analisis.StockProyectado,
                analisis.CantidadSugerida,
                analisis.NivelRiesgo,
                DiasHistorial = diasHistorial,
                DiasProyeccion = diasProyeccion
            });

        parameters.Add(
            "@IdModeloIA",
            1,
            DbType.Int32);

        parameters.Add(
            "@IdUsuario",
            idUsuario,
            DbType.Int32);

        parameters.Add(
            "@TipoPrediccion",
            "reposicion_stock",
            DbType.String,
            size: 100);

        parameters.Add(
            "@FechaInicioPeriodo",
            DateTime.Now.Date.AddDays(-diasHistorial),
            DbType.DateTime2);

        parameters.Add(
            "@FechaFinPeriodo",
            DateTime.Now.Date.AddDays(diasProyeccion),
            DbType.DateTime2);

        parameters.Add(
            "@ValorPredicho",
            analisis.DemandaProyectada,
            DbType.Decimal);

        parameters.Add(
            "@UnidadMedida",
            "unidades",
            DbType.String,
            size: 50);

        parameters.Add(
            "@NivelConfianza",
            CalcularConfianza(analisis),
            DbType.Decimal);

        parameters.Add(
            "@Parametros",
            parametrosJson,
            DbType.String);

        parameters.Add(
            "@IdPrediccionIA",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText:
                "ia.sp_prediccion_reposicion_guardar",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<long>(
            "@IdPrediccionIA");
    }

    public async Task<long> GuardarRecomendacionAsync(
        int idUsuario,
        RecomendacionReposicionResponse analisis,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "@IdUsuario",
            idUsuario,
            DbType.Int32);

        parameters.Add(
            "@IdModeloIA",
            1,
            DbType.Int32);

        parameters.Add(
            "@Modulo",
            "inventario",
            DbType.String,
            size: 100);

        parameters.Add(
            "@TipoRecomendacion",
            "reposicion",
            DbType.String,
            size: 100);

        parameters.Add(
            "@Titulo",
            $"Reposición recomendada - {analisis.Producto}",
            DbType.String,
            size: 250);

        parameters.Add(
            "@Descripcion",
            $"El producto {analisis.Producto} presenta un nivel de riesgo " +
            $"{analisis.NivelRiesgo}. " +
            $"Stock disponible: {analisis.StockDisponible:N2}. " +
            $"Demanda proyectada: {analisis.DemandaProyectada:N2}. " +
            $"Se recomienda reponer {analisis.CantidadSugerida:N2} unidades.",
            DbType.String);

        parameters.Add(
            "@Prioridad",
            ObtenerPrioridad(
                analisis.NivelRiesgo),
            DbType.String,
            size: 50);

        parameters.Add(
            "@ImpactoEstimado",
            $"Cantidad sugerida: {analisis.CantidadSugerida:N2} unidades.",
            DbType.String,
            size: 500);

        parameters.Add(
            "@Estado",
            "pendiente",
            DbType.String,
            size: 50);

        parameters.Add(
            "@IdRecomendacionIA",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText:
                "ia.sp_recomendacion_reposicion_guardar",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<long>(
            "@IdRecomendacionIA");
    }
    public async Task<IReadOnlyCollection<RecomendacionIAResponse>>
    ListarRecomendacionesAsync(
        int idUsuario,
        string? estado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ia.sp_recomendacion_listar",
            parameters: new
            {
                IdUsuario = idUsuario,
                Estado = estado
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<
                RecomendacionIAResponse>(
                    command);

        return resultado.ToList();
    }

    public async Task<RecomendacionIAResponse?>
        ObtenerRecomendacionPorIdAsync(
            long idRecomendacionIA,
            int idUsuario,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ia.sp_recomendacion_obtener",
            parameters: new
            {
                IdRecomendacionIA = idRecomendacionIA,
                IdUsuario = idUsuario
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QueryFirstOrDefaultAsync<
                RecomendacionIAResponse>(
                    command);
    }

    public async Task DescartarRecomendacionAsync(
        long idRecomendacionIA,
        int idUsuario,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "ia.sp_recomendacion_descartar",
            parameters: new
            {
                IdRecomendacionIA = idRecomendacionIA,
                IdUsuario = idUsuario
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
    private static decimal CalcularConfianza(
        RecomendacionReposicionResponse analisis)
    {
        if (analisis.ConsumoPeriodo <= 0)
        {
            return 0.60m;
        }

        if (analisis.ConsumoPeriodo > 20)
        {
            return 0.90m;
        }

        if (analisis.ConsumoPeriodo > 10)
        {
            return 0.80m;
        }

        return 0.70m;
    }

    private static string ObtenerPrioridad(
        string nivelRiesgo)
    {
        if (string.IsNullOrWhiteSpace(
                nivelRiesgo))
        {
            return "baja";
        }

        return nivelRiesgo
            .Trim()
            .ToUpperInvariant() switch
        {
            "CRITICO" => "urgente",
            "ALTO" => "alta",
            "MEDIO" => "media",
            _ => "baja"
        };
    }
}