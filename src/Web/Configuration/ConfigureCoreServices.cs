using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Logging;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.Web.Configuration;

public static class ConfigureCoreServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services,
        IConfiguration configuration)
    {
          
        services.Configure<DeliverySettingsOptions>(configuration.GetSection("DeliverySettings"));
        services.AddSingleton<IDeliverySettingsOptions>(sp =>
            sp.GetRequiredService<IOptions<DeliverySettingsOptions>>().Value);

        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBasketQueryService, BasketQueryService>();
        services.AddSingleton<IUriComposer>(new UriComposer(configuration.Get<CatalogSettings>()));
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddScoped<IOrderQueryService, OrderQueryService>();

        services.AddScoped<IDeliveryService, DeliveryService>();

        services.Configure<OrderItemReserverMessagingSettingsOptions>(
            (factory) => {
                factory.ServiceBusConnectionString = configuration.GetValue<string>("ServiceBusConnectionString");
                factory.OrderCompleteQueueName = configuration.GetValue<string>("OrderItemReserverMessagingSettingsOptions:OrderCompleteQueueName");
            }
        );
        services.AddSingleton<IOrderItemReserverMessagingSettingsOptions>(sp=> sp.GetRequiredService<IOptions<OrderItemReserverMessagingSettingsOptions>>().Value);
        services.AddScoped<IOrderItemReserverMessagingService, OrderItemReserverMessagingService>();

        return services;
    }
}
