using FindFi.Domain.Entities;

namespace FindFi.Dal.Abstractions;

public interface IOrderRepository
{
    Task<long> CreateAsync(Order entity, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Order?> GetWithItemsAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Order entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
