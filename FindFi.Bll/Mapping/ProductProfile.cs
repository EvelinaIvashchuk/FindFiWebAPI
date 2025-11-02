using AutoMapper;
using FindFi.Bll.DTOs;
using FindFi.Domain.Entities;

namespace FindFi.Bll.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>()
            .ForMember(d => d.Id, opt => opt.Ignore());
        CreateMap<UpdateProductDto, Product>()
            .ForMember(d => d.Id, opt => opt.Ignore());
    }
}