using AutoMapper;
using FindFi.Bll.Abstractions;
using FindFi.Bll.DTOs;
using FindFi.Dal.Abstractions;
using FindFi.Domain.Entities;

namespace FindFi.Bll.Services;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper) : IBookingService
{
    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await unitOfWork.Bookings.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<BookingDto>>(items);
    }

    public async Task<BookingDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await unitOfWork.Bookings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            throw new Domain.Exceptions.NotFoundException($"Booking {id} not found");
        return mapper.Map<BookingDto>(entity);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var entity = await unitOfWork.Bookings.GetCount(cancellationToken);
        return mapper.Map<int>(entity);
    }

    public async Task<int> CreateAsync(CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        Validate(dto);
        var entity = mapper.Map<Booking>(dto);
        var id = await unitOfWork.Bookings.CreateAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return id;
    }

    public async Task UpdateAsync(int id, UpdateBookingDto dto, CancellationToken cancellationToken = default)
    {
        Validate(dto);
        var existing = await unitOfWork.Bookings.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new Domain.Exceptions.NotFoundException($"Booking {id} not found");
        }

        existing.Status = dto.Status;
        existing.CheckInDate = dto.CheckInDate;
        existing.CheckOutDate = dto.CheckOutDate;
        existing.Currency = dto.Currency;
        existing.TotalAmount = dto.TotalAmount;

        var updated = await unitOfWork.Bookings.UpdateAsync(existing, cancellationToken);
        if (updated)
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
        else
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new Domain.Exceptions.BusinessConflictException($"Failed to update booking {id}");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await unitOfWork.Bookings.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new Domain.Exceptions.NotFoundException($"Booking {id} not found");
        }

        var deleted = await unitOfWork.Bookings.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
        else
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw new Domain.Exceptions.BusinessConflictException($"Failed to delete booking {id}");
        }
    }

    // C#
    private static void Validate(CreateBookingDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Обов’язкові ідентифікатори
        if (dto.ListingId <= 0)
            errors["ListingId"] = ["ListingId must be a positive number"];
        if (dto.GuestId <= 0)
            errors["GuestId"] = ["GuestId must be a positive number"];

        // Статус 0..3
        if (dto.Status > 3)
            errors["Status"] = ["Status must be between 0 and 3"];

        // Дати
        if (dto.CheckInDate == default)
            errors["CheckInDate"] = ["CheckInDate is required"];
        if (dto.CheckOutDate == default)
            errors["CheckOutDate"] = ["CheckOutDate is required"];
        if (dto.CheckInDate != default && dto.CheckOutDate != default && dto.CheckOutDate <= dto.CheckInDate)
            errors["DateRange"] = ["CheckOutDate must be greater than CheckInDate"];

        // Валюта
        if (string.IsNullOrWhiteSpace(dto.Currency))
            errors["Currency"] = ["Currency is required"];
        else if (dto.Currency.Length > 10)
            errors["Currency"] = ["Currency length must be <= 10 characters"];

        // Сума
        if (dto.TotalAmount < 0)
            errors["TotalAmount"] = ["TotalAmount must be non-negative"];

        if (errors.Count > 0)
            throw new Domain.Exceptions.ValidationException("Booking creation validation failed", errors);
    }

    private static void Validate(UpdateBookingDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Status: 0..3
        if (dto.Status > 3)
            errors["Status"] = ["Status must be between 0 and 3"];

        // Dates
        if (dto.CheckInDate == default)
            errors["CheckInDate"] = ["CheckInDate is required"];
        if (dto.CheckOutDate == default)
            errors["CheckOutDate"] = ["CheckOutDate is required"];
        if (dto.CheckInDate != default && dto.CheckOutDate != default && dto.CheckOutDate <= dto.CheckInDate)
            errors["DateRange"] = ["CheckOutDate must be greater than CheckInDate"];

        // Currency
        if (string.IsNullOrWhiteSpace(dto.Currency))
            errors["Currency"] = ["Currency is required"];
        else if (dto.Currency.Length > 10)
            errors["Currency"] = ["Currency length must be <= 10 characters"];

        // TotalAmount
        if (dto.TotalAmount < 0)
            errors["TotalAmount"] = ["TotalAmount must be non-negative"];

        if (errors.Count > 0)
            throw new Domain.Exceptions.ValidationException("Booking updating validation failed", errors);
    }
}