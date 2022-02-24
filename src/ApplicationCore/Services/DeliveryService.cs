using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using DeliveryCore.Models;

using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IOrderQueryService _orderQueryService;
    private readonly HttpClient _httpClient;
    private readonly IDeliverySettingsOptions _deliverySettings;
    private readonly string FunctionName = "SetOrder";
    private readonly string CodeParameterName = "code";
    public DeliveryService(
        IOrderQueryService orderQueryService,
        HttpClient httpClient,
        IDeliverySettingsOptions deliverySettings)
    {
        _orderQueryService = orderQueryService ?? throw new ArgumentNullException(nameof(orderQueryService));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _deliverySettings = deliverySettings ?? throw new ArgumentNullException(nameof(deliverySettings));
    }
    public async Task SetOrderAsync(int orderId)
    {
        var order = await GetOrderAsync(orderId);
        await CallFunctionAsync(MapToDeliveryOrder(order));
    }

    internal Task<Order> GetOrderAsync(int orderId)
    {
        return _orderQueryService.GetAsync(orderId);
    }

    internal Task CallFunctionAsync(DeliveryOrder deliveryOrder)
    {
        return _httpClient.PostAsync(
                    ComposeFunctionURL(),
                    ToJson(deliveryOrder));
    }

    internal string ComposeFunctionURL()
    {
        return $"{_deliverySettings.DeliveryBaseApiUrl}{FunctionName}?{CodeParameterName}={_deliverySettings.AuthKey}";
    }

    internal StringContent ToJson(object obj)
    {
        return new StringContent(
                        JsonSerializer.Serialize(obj, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), 
                        Encoding.UTF8, "application/json");
    }

    internal DeliveryOrder MapToDeliveryOrder(Order order)
    {
        return new DeliveryOrder { 
            City = order.ShipToAddress.City,
            Country = order.ShipToAddress.Country,
            OrderId = order.Id,
            State = order.ShipToAddress.State,
            Street = order.ShipToAddress.Street,
            ZipCode = order.ShipToAddress.ZipCode,
            Total = order.Total(),
            Products = new List<Product>
                        ( 
                            order.OrderItems.Select(c=> 
                                new Product { ProductId = c.Id.ToString(), Quantity = c.Units })
                        )
        };
    }
}
