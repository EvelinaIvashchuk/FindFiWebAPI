using FindFi.Dal.Abstractions;
using FindFi.Dal.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FindFi.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IConnectionFactory>(_ => new MySqlConnectionFactory(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Optional: allow direct injection of repositories; resolved from the active UnitOfWork scope
        services.AddScoped<IProductRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Products);
        services.AddScoped<ICustomerRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Customers);
        services.AddScoped<IOrderRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Orders);

        return services;
    }
}