using System.Data;
using Dapper;
using FindFi.Dal.Abstractions;
using FindFi.Domain.Entities;

namespace FindFi.Dal.Repositories;

public class ProductRepository(IDbConnection connection, IDbTransaction transaction) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = "SELECT Id, Name, Price FROM Product";
        return await connection.QueryAsync<Product>(new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT Id, Name, Price FROM Product WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Product>(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        var sql = @"INSERT INTO Product (Name, Price)
                    VALUES (@Name, @Price);
                    SELECT LAST_INSERT_ID();";
        var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { entity.Name, entity.Price }, transaction, cancellationToken: cancellationToken));
        entity.Id = id;
        return id;
    }

    public async Task<bool> UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        var sql = "UPDATE Product SET Name = @Name, Price = @Price WHERE Id = @Id";
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { entity.Name, entity.Price, entity.Id }, transaction, cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM Product WHERE Id = @Id";
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken));
        return affected > 0;
    }
}