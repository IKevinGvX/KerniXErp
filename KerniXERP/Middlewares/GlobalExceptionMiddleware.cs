using System.Net;
using System.Text.Json;
using KernixERP.DTOs.Common;
using Microsoft.Data.SqlClient;

namespace KernixERP.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning(
                "La solicitud fue cancelada por el cliente. TraceId: {TraceId}",
                context.TraceIdentifier);

            if (!context.Response.HasStarted)
            {
                await EscribirRespuestaAsync(
                    context,
                    StatusCodes.Status499ClientClosedRequest,
                    "La solicitud fue cancelada.",
                    null);
            }
        }
        catch (Exception exception)
        {
            await ManejarExcepcionAsync(
                context,
                exception);
        }
    }

    private async Task ManejarExcepcionAsync(
        HttpContext context,
        Exception exception)
    {
        var detalle = ResolverExcepcion(exception);

        RegistrarError(
            context,
            exception,
            detalle.StatusCode);

        if (context.Response.HasStarted)
        {
            _logger.LogWarning(
                "No se pudo escribir la respuesta de error porque la respuesta HTTP ya había iniciado. TraceId: {TraceId}",
                context.TraceIdentifier);

            return;
        }

        await EscribirRespuestaAsync(
            context,
            detalle.StatusCode,
            detalle.Message,
            detalle.Errors);
    }

    private ErrorHttp ResolverExcepcion(
        Exception exception)
    {
        return exception switch
        {
            SqlException sqlException =>
                ResolverSqlException(sqlException),

            ArgumentException argumentException =>
                new ErrorHttp(
                    StatusCodes.Status400BadRequest,
                    argumentException.Message),

            KeyNotFoundException keyNotFoundException =>
                new ErrorHttp(
                    StatusCodes.Status404NotFound,
                    keyNotFoundException.Message),

            UnauthorizedAccessException unauthorizedException =>
                new ErrorHttp(
                    StatusCodes.Status401Unauthorized,
                    unauthorizedException.Message),

            InvalidOperationException invalidOperationException =>
                new ErrorHttp(
                    StatusCodes.Status409Conflict,
                    invalidOperationException.Message),

            TimeoutException =>
                new ErrorHttp(
                    StatusCodes.Status408RequestTimeout,
                    "La operación excedió el tiempo máximo permitido."),

            _ =>
                new ErrorHttp(
                    StatusCodes.Status500InternalServerError,
                    _environment.IsDevelopment()
                        ? exception.Message
                        : "Ocurrió un error interno en el servidor.")
        };
    }

    private ErrorHttp ResolverSqlException(
     SqlException exception)
    {
        // ======================================================
        // Restricción UNIQUE / PK duplicada
        // ======================================================

        if (exception.Number is 2601 or 2627)
        {
            return new ErrorHttp(
                StatusCodes.Status409Conflict,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "Ya existe un registro con los mismos datos.");
        }

        // ======================================================
        // FOREIGN KEY / CHECK
        // ======================================================

        if (exception.Number == 547)
        {
            return new ErrorHttp(
                StatusCodes.Status409Conflict,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "La operación viola una relación o restricción de datos.");
        }

        // ======================================================
        // Campo NOT NULL
        // ======================================================

        if (exception.Number == 515)
        {
            return new ErrorHttp(
                StatusCodes.Status400BadRequest,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "Falta proporcionar un dato obligatorio.");
        }

        // ======================================================
        // Texto demasiado largo
        // ======================================================

        if (exception.Number is 8152 or 2628)
        {
            return new ErrorHttp(
                StatusCodes.Status400BadRequest,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "Uno de los valores supera la longitud permitida.");
        }

        // ======================================================
        // Parámetro requerido no enviado
        // ======================================================

        if (exception.Number == 201)
        {
            return new ErrorHttp(
                StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "Existe una inconsistencia entre la API y el procedimiento almacenado.");
        }

        // ======================================================
        // Parámetro adicional/no reconocido
        // ======================================================

        if (exception.Number == 8144)
        {
            return new ErrorHttp(
                StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "Existe una inconsistencia entre los parámetros de la API y SQL Server.");
        }

        // ======================================================
        // Tabla / SP / objeto inexistente
        // ======================================================

        if (exception.Number == 208)
        {
            return new ErrorHttp(
                StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "No se encontró un objeto requerido en SQL Server.");
        }

        // ======================================================
        // Columna inexistente
        // ======================================================

        if (exception.Number == 207)
        {
            return new ErrorHttp(
                StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "La estructura de la base de datos no coincide con la API.");
        }

        // ======================================================
        // Error de conversión
        // ======================================================

        if (exception.Number is 245 or 8114)
        {
            return new ErrorHttp(
                StatusCodes.Status400BadRequest,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "Uno de los datos tiene un tipo o formato incorrecto.");
        }

        // ======================================================
        // Timeout
        // ======================================================

        if (exception.Number == -2)
        {
            return new ErrorHttp(
                StatusCodes.Status408RequestTimeout,
                _environment.IsDevelopment()
                    ? $"SQL {exception.Number}: {exception.Message}"
                    : "SQL Server tardó demasiado en responder.");
        }

        // ======================================================
        // THROW personalizados de KerniX
        // ======================================================

        if (exception.Number >= 50000)
        {
            return new ErrorHttp(
                StatusCodes.Status400BadRequest,
                exception.Message);
        }

        // ======================================================
        // Desarrollo: mostrar error SQL completo
        // Producción: ocultarlo
        // ======================================================

        return new ErrorHttp(
            StatusCodes.Status500InternalServerError,
            _environment.IsDevelopment()
                ? $"SQL {exception.Number} | Procedimiento: {exception.Procedure} | Línea: {exception.LineNumber} | {exception.Message}"
                : "Ocurrió un error al procesar la operación en la base de datos.");
    }
    private void RegistrarError(
        HttpContext context,
        Exception exception,
        int statusCode)
    {
        var idUsuario =
            context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var idEmpleado =
            context.User.FindFirst("id_empleado")?.Value;

        var idEmpresa =
            context.User.FindFirst("id_empresa")?.Value;

        var idSucursal =
            context.User.FindFirst("id_sucursal")?.Value;

        _logger.LogError(
            exception,
            """
            Error no controlado en la API.
            StatusCode: {StatusCode}
            TraceId: {TraceId}
            Método: {Metodo}
            Ruta: {Ruta}
            QueryString: {QueryString}
            IP: {Ip}
            Usuario: {IdUsuario}
            Empleado: {IdEmpleado}
            Empresa: {IdEmpresa}
            Sucursal: {IdSucursal}
            """,
            statusCode,
            context.TraceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString.Value,
            context.Connection.RemoteIpAddress?.ToString(),
            idUsuario,
            idEmpleado,
            idEmpresa,
            idSucursal);
    }

    private static async Task EscribirRespuestaAsync(
        HttpContext context,
        int statusCode,
        string message,
        object? errors)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType =
            "application/json; charset=utf-8";

        var response = new
        {
            success = false,
            message,
            data = (object?)null,
            errors,
            timestamp = DateTime.UtcNow,
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json);
    }

    private sealed record ErrorHttp(
        int StatusCode,
        string Message,
        object? Errors = null);
}