using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace KernixERP.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public HealthController(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            api = "KernixERP API",
            status = "online",
            fecha = DateTime.UtcNow
        }));
    }

    [HttpGet("database")]
    public async Task<IActionResult> Database(
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            select
                db_name() as base_datos,
                @@servername as servidor,
                sysdatetime() as fecha_servidor;
            """;

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        var result = await connection.QuerySingleAsync<object>(command);

        return Ok(ApiResponse<object>.Ok(
            result,
            "Conexión con SQL Server correcta."));
    }
}