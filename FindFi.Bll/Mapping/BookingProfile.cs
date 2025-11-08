using AutoMapper;
using FindFi.Bll.DTOs;
using FindFi.Domain.Entities;

namespace FindFi.Bll.Mapping;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingDto>();
        CreateMap<CreateBookingDto, Booking>()
            .ForMember(d => d.Id, opt => opt.Ignore());
        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(d => d.Id, opt => opt.Ignore());
    }
}