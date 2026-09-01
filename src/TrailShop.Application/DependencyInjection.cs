using Microsoft.Extensions.DependencyInjection;
using TrailShop.Application.Catalog;
using TrailShop.Application.Orders;
using TrailShop.Domain.Pricing;

namespace TrailShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<PricingCalculator>();
        services.AddScoped<CatalogService>();
        services.AddScoped<OrderService>();
        services.AddScoped<OrderSummaryBuilder>();
        return services;
    }
}
