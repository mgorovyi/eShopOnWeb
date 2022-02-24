using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderItemReserverService : IOrderItemReserverService
{
    private readonly HttpClient _httpClient;
    private readonly OrderItemReserverSettingsOptions _warehouseSettings;
    public OrderItemReserverService(HttpClient httpClient, OrderItemReserverSettingsOptions  warehouseSettings)
    {
        this._httpClient = httpClient;
        this._warehouseSettings = warehouseSettings;
    }
    public async Task SendItemRequestAsync(Dictionary<string, int> items)
    {
        var record = items.Select(x=> new { id = x.Key, quantity = x.Value});
        var content = ToJson(record);
        var url = $"{_warehouseSettings.WarehouseBaseApiUrl}OrderItemsReserver?code={_warehouseSettings.AuthKey}";
        var result = await _httpClient.PostAsync( url, content);
    }

    private StringContent ToJson(object obj)
    {
        return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
    }
}
