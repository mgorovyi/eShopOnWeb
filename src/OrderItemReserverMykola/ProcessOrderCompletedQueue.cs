using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OrderItemReserverMykola;

public static class ProcessOrderCompletedQueue
{
    //https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-error-pages?tabs=csharp
    // The default Service Bus queue and topic policy will write a message to a dead-letter queue after 10 attempts.
    //[FixedDelayRetry(5, "00:00:10")]
    [FunctionName("OrderItemReserverMykola")]
    public static async Task Run([ServiceBusTrigger("ordercompleted", Connection = "ServiceBusConnectionString")] string myQueueItem,
        [Blob("orders", Connection = "OrderItemReserverStorageConnectionString")] BlobContainerClient blobContainerClient,
        ILogger log)
    {
        log.LogWarning($"Proceed message: {myQueueItem}");

        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(GetJsonRandomFileName());


        var buffer = Encoding.UTF8.GetBytes(myQueueItem);

        using (var stream = new MemoryStream(buffer))
        {
            await blobClient.UploadAsync(stream);
        }
      
    }
    static string GetJsonRandomFileName()
    {
        return $"{Guid.NewGuid().ToString().Trim(new char[] { '{', '}' })}.json";
    }
}
