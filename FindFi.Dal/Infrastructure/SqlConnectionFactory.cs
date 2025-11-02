using System.Data;
using MySqlConnector;

namespace FindFi.Dal.Infrastructure;

public class MySqlConnectionFactory(string connectionString) : IConnectionFactory
{
    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var conn = new MySqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        return conn;
    }
}