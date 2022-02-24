namespace WarehouseFunctionApp;

public interface IOrderItemReserverTaskOptions
{
    public string StorageAccountName { get; }
    public string ContainerName { get; }
    public string ManagedIdentityClientId { get; }
}
public class OrderItemReserverTaskOptions : IOrderItemReserverTaskOptions
{
    public string StorageAccountName { get; set; }
    public string ContainerName { get; set; }
    public string ManagedIdentityClientId { get; set; }
}
