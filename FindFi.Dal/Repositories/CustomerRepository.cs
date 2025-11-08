using System.Data;
using System.Data.Common;
using FindFi.Dal.Abstractions;
using FindFi.Domain.Entities;

namespace FindFi.Dal.Repositories;

// Pure ADO.NET repository (no Dapper)
public class CustomerRepository(IDbConnection connection, IDbTransaction transaction) : ICustomerRepository
{
    private readonly DbConnection _connection = (DbConnection)connection;
    private readonly DbTransaction _transaction = (DbTransaction)transaction;

    public async Task<long> CreateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = @"INSERT INTO Customer (Email, FullName)
                            VALUES (@Email, @FullName);
                            SELECT LAST_INSERT_ID();";
        AddParam(cmd, "@Email", entity.Email);
        AddParam(cmd, "@FullName", entity.FullName);

        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        var id = Convert.ToInt64(result);
        entity.Id = id;
        return id;
    }

    public async Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = @"SELECT CustomerId, Email, FullName
                             FROM Customer WHERE CustomerId = @Id";
        AddParam(cmd, "@Id", id);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) return null;
        return Map(reader);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = @"SELECT CustomerId, Email, FullName
                             FROM Customer WHERE Email = @Email";
        AddParam(cmd, "@Email", email);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) return null;
        return Map(reader);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = @"SELECT CustomerId, Email, FullName, CreatedAt FROM Customer";
        var list = new List<Customer>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            list.Add(Map(reader));
        }
        return list;
    }

    public async Task<bool> UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = @"UPDATE Customer SET FullName = @FullName, Email = @Email,
                             WHERE CustomerId = @Id";
        AddParam(cmd, "@FullName", entity.FullName);
        AddParam(cmd, "@Email", entity.Email);

        var affected = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var cmd = _connection.CreateCommand();
        cmd.Transaction = _transaction;
        cmd.CommandText = @"DELETE FROM Customer WHERE CustomerId = @Id";
        AddParam(cmd, "@Id", id);
        var affected = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return affected > 0;
    }

    private static void AddParam(DbCommand cmd, string name, object? value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(p);
    }

    private static Customer Map(DbDataReader reader)
    {
        return new Customer
        {
            Id = reader.GetInt64(reader.GetOrdinal("Id")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}
