namespace Microsoft.eShopWeb.ApplicationCore;

public class OrderItemReserverSettingsOptions
{
    public const string OrderItemReserverSettings = "OrderItemReserverSettings";
    public string WarehouseBaseApiUrl { get; set; }
    public string AuthKey { get; set; }
}
