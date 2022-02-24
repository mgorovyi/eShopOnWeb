using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeliveryCore.Models;

using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Exceptions;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Infrastructure.Services;

using Moq;

using Xunit;
namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.WarehouseTests;

public class WarehouseMessagingServiceTests
{
    private const string ConnectionString = "connectionString";
    private const string QueueName = "queueName";

    Mock<IAppLogger<OrderItemReserverMessagingService>> _loggerMoq = new Mock<IAppLogger<OrderItemReserverMessagingService>>();

    [Fact]
    public void CtorShouldThrowArgumentNullExceptionIfSettingsAreNull()
    {
        OrderItemReserverMessagingSettingsOptions settings = null;
        Assert.Throws<ArgumentNullException>(() => new OrderItemReserverMessagingService(settings,_loggerMoq.Object));
    }

    [Fact]
    public void IsProductsEmptyShouldReturnTrue()
    {
        var service = new OrderItemReserverMessagingService(CreateValidSettings(), _loggerMoq.Object);
        Assert.True(service.IsProductsEmpty(new List<Product>()));
    }

    [Fact]
    public async Task SendOrderCompleteMessageThrowArgumentNullException()
    {
        OrderItemReserverMessagingService service = new OrderItemReserverMessagingService(CreateValidSettings(), _loggerMoq.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(
           async () => await service.SendOrderCompleteMessageAsync(null));
    }

    [Fact]
    public async Task SendOrderCompleteMessageThrowProductsEmptyException()
    {
        OrderItemReserverMessagingService service = new OrderItemReserverMessagingService(CreateValidSettings(), _loggerMoq.Object);
        await Assert.ThrowsAsync<ProductsEmptyException>(
           async () => await service.SendOrderCompleteMessageAsync(new List<Product>()));
    }

    IOrderItemReserverMessagingSettingsOptions CreateSettings(string connectionString, string queueName)
    {
        return new OrderItemReserverMessagingSettingsOptions { 
            OrderCompleteQueueName = queueName,
            ServiceBusConnectionString = connectionString
        };
    }

    IOrderItemReserverMessagingSettingsOptions CreateValidSettings()
    {
        return CreateSettings(ConnectionString, QueueName);
    }
}
