using System.Data;
using FindFi.Dal.Abstractions;
using FindFi.Dal.Infrastructure;
using FindFi.Dal.Repositories;

namespace FindFi.Dal;

/// <summary>
/// Unit of Work coordinates a single open connection and transaction across multiple repositories.
/// Default isolation level: ReadCommitted (sane default for OLTP). Consider raising to RepeatableRead for
/// stricter consistency at the cost of higher deadlock probability. Keep commands short to reduce contention.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IConnectionFactory _connectionFactory;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private IBookingRepository? _bookings;
    private ICustomerRepository? _customers;
    private bool _disposed;

    public UnitOfWork(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        // Open connection and begin transaction immediately for the unit-of-work scope
        _connection = _connectionFactory.CreateOpenConnectionAsync().GetAwaiter().GetResult();
        _transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
    }

    public IBookingRepository Bookings => _bookings ??= new BookingRepository(Connection, Transaction);

    public ICustomerRepository Customers => _customers ??= new CustomerRepository(Connection, Transaction);
    //public IOrderRepository Orders => _orders ??= new OrderRepository(Connection, Transaction);

    public IDbConnection Connection => _connection ?? throw new ObjectDisposedException(nameof(UnitOfWork));
    public IDbTransaction Transaction => _transaction ?? throw new ObjectDisposedException(nameof(UnitOfWork));

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        Transaction.Commit();
        ResetTransaction();
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        Transaction.Rollback();
        ResetTransaction();
        return Task.CompletedTask;
    }

    private void ResetTransaction()
    {
        _transaction?.Dispose();
        if (_connection is { State: ConnectionState.Open })
        {
            _transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
            // reset repositories to ensure they use a fresh transaction
            _bookings = null;
            _customers = null;
            //_orders = null;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _transaction?.Dispose();
        _connection?.Dispose();
        _transaction = null;
        _connection = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        if (_transaction is IAsyncDisposable asyncTx)
            await asyncTx.DisposeAsync();
        else
            _transaction?.Dispose();

        if (_connection is IAsyncDisposable asyncConn)
            await asyncConn.DisposeAsync();
        else
            _connection?.Dispose();

        _transaction = null;
        _connection = null;
    }
}