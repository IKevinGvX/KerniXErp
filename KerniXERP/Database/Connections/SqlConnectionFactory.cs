using Microsoft.Data.SqlClient;
using System.Data;

namespace KernixERP.Database.Connections;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KernixERP")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión 'KernixERP'.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}