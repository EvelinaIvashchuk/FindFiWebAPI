using FindFi.Domain.Entities;

namespace FindFi.Dal.Abstractions;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Product entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Product entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}