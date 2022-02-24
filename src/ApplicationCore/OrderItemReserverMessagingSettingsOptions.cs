namespace Microsoft.eShopWeb.ApplicationCore;

public interface IOrderItemReserverMessagingSettingsOptions
{
    public string ServiceBusConnectionString { get;}
    public string OrderCompleteQueueName { get; }
}
public class OrderItemReserverMessagingSettingsOptions : IOrderItemReserverMessagingSettingsOptions
{
    public string ServiceBusConnectionString { get; set; }
    public string OrderCompleteQueueName { get; set; }
}
