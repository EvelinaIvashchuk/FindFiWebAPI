using System.Data;
using Dapper;
using FindFi.Dal.Abstractions;
using FindFi.Domain.Entities;

namespace FindFi.Dal.Repositories;

public class OrderRepository(IDbConnection connection, IDbTransaction transaction) : IOrderRepository
{
    public async Task<long> CreateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"INSERT INTO `Order` (CustomerId, Status, Currency, TotalAmount, PlacedAt)
                    VALUES (@CustomerId, @Status, @Currency, @TotalAmount, @PlacedAt);
                    SELECT LAST_INSERT_ID();";
        var id = await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, new
        {
            entity.CustomerId,
            entity.Status,
            entity.Currency,
            entity.TotalAmount,
            entity.PlacedAt
        }, transaction, cancellationToken: cancellationToken));
        entity.OrderId = id;
        return id;
    }

    public async Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT OrderId, CustomerId, Status, Currency, TotalAmount, PlacedAt, CreatedAt
                    FROM `Order`
                    WHERE OrderId = @Id";
        return await connection.QuerySingleOrDefaultAsync<Order>(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken));
    }

    public async Task<Order?> GetWithItemsAsync(long id, CancellationToken cancellationToken = default)
    {
        var orderDict = new Dictionary<long, Order>();
        const string sql = @"SELECT o.OrderId, o.CustomerId, o.Status, o.Currency, o.TotalAmount, o.PlacedAt, o.CreatedAt,
                           oi.OrderItemId, oi.OrderId, oi.ProductId, oi.UnitPrice, oi.Quantity, oi.LineTotal
                    FROM `Order` o
                    LEFT JOIN OrderItem oi ON o.OrderId = oi.OrderId
                    WHERE o.OrderId = @Id";
        await connection.QueryAsync<Order, OrderItem, Order>(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken),
            (o, oi) =>
            {
                if (!orderDict.TryGetValue(o.OrderId, out var entry))
                {
                    entry = o;
                    entry.Items = new List<OrderItem>();
                    orderDict.Add(entry.OrderId, entry);
                }
                if (oi != null && oi.OrderItemId != 0)
                {
                    entry.Items.Add(oi);
                }
                return entry;
            }, splitOn: "OrderItemId");

        return orderDict.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT OrderId, CustomerId, Status, Currency, TotalAmount, PlacedAt, CreatedAt FROM `Order`";
        return await connection.QueryAsync<Order>(new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        const string sql = @"UPDATE `Order` SET Status = @Status, Currency = @Currency, TotalAmount = @TotalAmount, PlacedAt = @PlacedAt
                    WHERE OrderId = @OrderId";
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            entity.Status,
            entity.Currency,
            entity.TotalAmount,
            entity.PlacedAt,
            entity.OrderId
        }, transaction, cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM `Order` WHERE OrderId = @Id";
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken));
        return affected > 0;
    }
}
