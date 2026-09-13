using System.Data;
using System.Text.Json;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Models.IA;

namespace KernixERP.Repositories.IA;

public sealed class PronosticoVentasRepository
    : IPronosticoVentasRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public PronosticoVentasRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // ==========================================================
    // ANALIZAR PRONÓSTICO DE VENTAS
    // ==========================================================

    public async Task<IReadOnlyCollection<PronosticoVentaResponse>> AnalizarAsync(
        int idEmpresa,
        PronosticoVentaRequest request,
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
            "@id_producto",
            request.IdProducto,
            DbType.Int32);

        parameters.Add(
            "@dias_historial",
            request.DiasHistorial,
            DbType.Int32);

        parameters.Add(
            "@dias_proyeccion",
            request.DiasProyeccion,
            DbType.Int32);

        var command = new CommandDefinition(
            "ia.sp_pronostico_ventas_analizar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<PronosticoVentaResponse>(
                command);

        return resultado.ToList();
    }

    // ==========================================================
    // GUARDAR PREDICCIÓN IA
    // ==========================================================

    public async Task<long> GuardarPrediccionAsync(
        int idModeloIA,
        int idUsuario,
        PronosticoVentaResponse pronostico,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var fechaInicio =
            DateTime.Today.AddDays(1);

        var fechaFin =
            fechaInicio.AddDays(
                pronostico.DiasProyeccion - 1);

        var nivelConfianza =
            CalcularNivelConfianza(
                pronostico.NumeroVentas);

        var parametros =
            JsonSerializer.Serialize(
                new
                {
                    pronostico.IdProducto,
                    pronostico.CodigoProducto,
                    pronostico.Producto,

                    pronostico.CantidadVendidaHistorica,
                    pronostico.PromedioVentaDiaria,

                    pronostico.VentasPeriodoAnterior,
                    pronostico.VentasPeriodoReciente,
                    pronostico.Tendencia,

                    pronostico.ImporteHistorico,
                    pronostico.NumeroVentas,

                    pronostico.DiasHistorial,
                    pronostico.DiasProyeccion,

                    pronostico.FechaInicioHistorial,
                    pronostico.FechaFinHistorial
                });

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "@id_modelo_ia",
            idModeloIA,
            DbType.Int32);

        parameters.Add(
            "@id_usuario",
            idUsuario,
            DbType.Int32);

        parameters.Add(
            "@fecha_inicio_periodo",
            fechaInicio.Date,
            DbType.Date);

        parameters.Add(
            "@fecha_fin_periodo",
            fechaFin.Date,
            DbType.Date);

        parameters.Add(
            "@valor_predicho",
            pronostico.CantidadPronosticada,
            DbType.Decimal);

        parameters.Add(
            "@unidad_medida",
            "unidades",
            DbType.String);

        parameters.Add(
            "@nivel_confianza",
            nivelConfianza,
            DbType.Decimal);

        parameters.Add(
            "@parametros",
            parametros,
            DbType.String);

        parameters.Add(
            "@id_prediccion_ia",
            dbType: DbType.Int64,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            "ia.sp_prediccion_ventas_guardar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<long>(
            "@id_prediccion_ia");
    }

    // ==========================================================
    // LISTAR PREDICCIONES DE VENTAS
    // ==========================================================

    public async Task<IReadOnlyCollection<PrediccionIA>>
        ListarPrediccionesAsync(
            int idUsuario,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "@id_usuario",
            idUsuario,
            DbType.Int32);

        parameters.Add(
            "@id_modelo_ia",
            null,
            DbType.Int32);

        var command = new CommandDefinition(
            "ia.sp_prediccion_ventas_listar",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var resultado =
            await connection.QueryAsync<PrediccionIA>(
                command);

        return resultado.ToList();
    }

    // ==========================================================
    // NIVEL DE CONFIANZA
    // ==========================================================

    private static decimal CalcularNivelConfianza(
        int numeroVentas)
    {
        return numeroVentas switch
        {
            >= 30 => 0.90m,
            >= 15 => 0.80m,
            >= 5 => 0.70m,
            _ => 0.60m
        };
    }
}