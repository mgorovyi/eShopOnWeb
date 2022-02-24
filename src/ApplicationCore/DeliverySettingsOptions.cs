namespace Microsoft.eShopWeb.ApplicationCore;

public interface IDeliverySettingsOptions
{
    public string DeliveryBaseApiUrl { get; }
    public string AuthKey { get;  }
}

public class DeliverySettingsOptions : IDeliverySettingsOptions
{
    public string DeliveryBaseApiUrl { get; set; }
    public string AuthKey { get; set; }
}
