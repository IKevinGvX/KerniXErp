using Microsoft.Data.SqlClient;
using System.Data;

namespace KernixERP.Database.Connections;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}