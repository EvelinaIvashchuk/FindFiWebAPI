using AutoMapper;
using FindFi.Bll.Abstractions;
using FindFi.Bll.DTOs;
using FindFi.Dal.Abstractions;
using FindFi.Domain.Entities;

namespace FindFi.Bll.Services;

public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await unitOfWork.Products.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<ProductDto>>(items);
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new Domain.Exceptions.NotFoundException($"Product {id} not found");
        return mapper.Map<ProductDto>(entity);
    }

    public async Task<int> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        Validate(dto);
        var entity = mapper.Map<Product>(dto);
        var id = await unitOfWork.Products.CreateAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return id;
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        Validate(dto);
        var existing = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new Domain.Exceptions.NotFoundException($"Product {id} not found");
        }

        existing.Name = dto.Name;
        existing.Price = dto.Price;

        var updated = await unitOfWork.Products.UpdateAsync(existing, cancellationToken);
        if (updated)
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
        else
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new Domain.Exceptions.BusinessConflictException($"Failed to update product {id}");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new Domain.Exceptions.NotFoundException($"Product {id} not found");
        }

        var deleted = await unitOfWork.Products.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
        else
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new Domain.Exceptions.BusinessConflictException($"Failed to delete product {id}");
        }
    }

    private static void Validate(CreateProductDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors["Name"] = ["Name is required"]; 
        else if (dto.Name.Length > 200)
            errors["Name"] = ["Name length must be <= 200 characters"]; 
        if (dto.Price < 0)
            errors["Price"] = ["Price must be non-negative"]; 

        if (errors.Count > 0)
            throw new Domain.Exceptions.ValidationException("Product validation failed", errors);
    }

    private static void Validate(UpdateProductDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors["Name"] = ["Name is required"]; 
        else if (dto.Name.Length > 200)
            errors["Name"] = ["Name length must be <= 200 characters"]; 
        if (dto.Price < 0)
            errors["Price"] = ["Price must be non-negative"]; 

        if (errors.Count > 0)
            throw new Domain.Exceptions.ValidationException("Product validation failed", errors);
    }
}