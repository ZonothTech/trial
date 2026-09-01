using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrailShop.Application.Abstractions;
using TrailShop.Infrastructure.Data;
using TrailShop.Infrastructure.Export;
using TrailShop.Infrastructure.Payments;
using TrailShop.Infrastructure.Reporting;
using TrailShop.Infrastructure.Repositories;

namespace TrailShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var shopConnection = configuration.GetConnectionString("Shop") ?? "Data Source=trailshop.db";

        services.AddDbContext<ShopDbContext>(options => options.UseSqlite(shopConnection));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IPaymentGateway, FakePaymentGatewayClient>();
        services.AddSingleton<LegacyCatalogExporter>();
        services.AddSingleton(new ReportingSettings(configuration.GetConnectionString("Reporting") ?? string.Empty));

        return services;
    }
}
