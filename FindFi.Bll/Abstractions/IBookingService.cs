using FindFi.Bll.DTOs;

namespace FindFi.Bll.Abstractions;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookingDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateBookingDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateBookingDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}