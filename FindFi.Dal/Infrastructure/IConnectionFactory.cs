using System.Data;

namespace FindFi.Dal.Infrastructure;

public interface IConnectionFactory
{
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}