using FindFi.Domain.Entities;

namespace FindFi.Dal.Abstractions;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> GetCount(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Booking entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Booking entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}