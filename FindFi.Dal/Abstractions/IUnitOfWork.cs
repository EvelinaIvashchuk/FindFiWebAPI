using System.Data;

namespace FindFi.Dal.Abstractions;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    // Repositories
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    IOrderRepository Orders { get; }

    // Transaction control
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);

    // Expose low-level objects if needed (read-only)
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
}