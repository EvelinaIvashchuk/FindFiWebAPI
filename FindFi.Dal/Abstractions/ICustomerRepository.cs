using FindFi.Domain.Entities;

namespace FindFi.Dal.Abstractions;

public interface ICustomerRepository
{
    Task<long> CreateAsync(Customer entity, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Customer entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
