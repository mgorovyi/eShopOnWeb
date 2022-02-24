using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DeliveryCore.Models;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class OrderItemReserverMessagingService : IOrderItemReserverMessagingService
{
    private readonly IOrderItemReserverMessagingSettingsOptions _settings;
    private readonly IAppLogger<OrderItemReserverMessagingService> _logger;

    public OrderItemReserverMessagingService(
		IOrderItemReserverMessagingSettingsOptions settings,
		IAppLogger<OrderItemReserverMessagingService> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendOrderCompleteMessageAsync(IList<Product> products)
    {
        _ = products ?? throw new ArgumentNullException(nameof(products));

        if (IsProductsEmpty(products)) throw new ProductsEmptyException();

        await using var client = new ServiceBusClient(_settings.ServiceBusConnectionString);
        await using var queue = client.CreateSender(_settings.OrderCompleteQueueName);
        try
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(ToJson(products)));

            await queue.SendMessageAsync(message);
        }
        catch(Exception e)
        {
            _logger.LogError("Sending message failed.", e);
        }
        finally
        {
            await queue.DisposeAsync();
            await client.DisposeAsync();
        }
    }

    public bool IsProductsEmpty(IList<Product> products)
    {
        return products?.Count == 0;
    }

    private string ToJson(IList<Product> products)
    {
        return JsonSerializer.Serialize(products);
    }
}
