using System.Data;
using Dapper;
using FindFi.Dal.Abstractions;
using FindFi.Domain.Entities;

namespace FindFi.Dal.Repositories;

public class BookingRepository(IDbConnection connection, IDbTransaction transaction) : IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Booking";
        return await connection.QueryAsync<Booking>(new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken));
    }
    
    public async Task<int> GetCount(CancellationToken cancellationToken = default)
    {
        var sql = "SELECT COUNT(*) FROM Booking";
        return await connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Booking?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Booking WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Booking>(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        var sql = @"INSERT INTO Booking (ListingId, GuestId, Status, CheckInDate, CheckOutDate, Currency, TotalAmount)
                        VALUES (@ListingId, @GuestId, @Status, @CheckInDate, @CheckOutDate, @Currency, @TotalAmount);
                        SELECT LAST_INSERT_ID();";
        var id = await connection.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, new
            {
                entity.ListingId,
                entity.GuestId,
                entity.Status,
                entity.CheckInDate,
                entity.CheckOutDate,
                entity.Currency,
                entity.TotalAmount
            }, transaction, cancellationToken: cancellationToken));

        entity.Id = id;
        return (int)id;
    }

    public async Task<bool> UpdateAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        var sql = @"UPDATE Booking SET Status = @Status, CheckInDate = @CheckInDate,CheckOutDate = @CheckOutDate, 
                   Currency = @Currency, TotalAmount = @TotalAmount WHERE Id = @Id";
        var affected = await connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                entity.Status,
                entity.CheckInDate,
                entity.CheckOutDate,
                entity.Currency,
                entity.TotalAmount,
                entity.Id
            }, transaction, cancellationToken: cancellationToken));
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM Booking WHERE Id = @Id";
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, transaction, cancellationToken: cancellationToken));
        return affected > 0;
    }
}