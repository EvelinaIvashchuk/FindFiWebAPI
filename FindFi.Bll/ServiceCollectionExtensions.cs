using FindFi.Bll.Abstractions;
using FindFi.Bll.Mapping;
using FindFi.Bll.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FindFi.Bll;

public static class ServiceCollectionExtensions
{
    public static void AddBll(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        }, typeof(ProductProfile).Assembly);

        services.AddScoped<IProductService, ProductService>();
    }
}