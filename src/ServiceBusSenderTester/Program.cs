// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

Console.WriteLine("Hello, World!");


await using var client = new ServiceBusClient(
    "Endpoint=sb://vive0110theresbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=72+zQX5DBRod0/Uc5alvT0IPz7KOXzDCXnYmD+8Elhc="
    //, this is  default options. no need to overwrite. just example of usage
    //new ServiceBusClientOptions() {
    //RetryOptions = new ServiceBusRetryOptions { 
    //    Mode = ServiceBusRetryMode.Exponential,
    //    MaxRetries = 3,
    //    MaxDelay = TimeSpan.FromSeconds(60)
    //    }
    //}
    );
await using var queue = client.CreateSender("ordercompleted");
try
{
    int count = 0;
    Dictionary<string, int> products = new Dictionary<string, int>();
    products.Add("1", 1);
    products.Add("2", 2);
    products.Add("3", 3);
    products.Add("4", 4);

    var json = JsonSerializer.Serialize(products.Select(x=> new { 
        productId = x.Key,
        quantity = x.Value
    }));

    //request logic app to send email
    var body = new { products = json , type="string"};
    var contentBody = JsonSerializer.Serialize(body);

    while (count++ < 2)
    {
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(contentBody));
        message.ContentType = "application/json";
        
        await queue.SendMessageAsync(message);

        await Task.Delay(100);

        Console.WriteLine($"Sent message {count}");
    }
}
finally
{
    await queue.DisposeAsync();
    await client.DisposeAsync();
}
