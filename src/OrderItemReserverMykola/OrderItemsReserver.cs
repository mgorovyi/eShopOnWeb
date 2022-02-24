using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using Azure.Storage.Blobs;

namespace OrderItemReserverMykola;

public static class OrderItemsReserver
{
    [ExponentialBackoffRetry(5, "00:00:04", "00:15:00")]
    [FunctionName("OrderItemsReserver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [Blob("orders", Connection = "OrderItemReserverStorageConnectionString")] BlobContainerClient blobContainerClient,
        ILogger log
        )
    {
        try
        {
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(GetJsonRandomFileName());

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var buffer = Encoding.UTF8.GetBytes(requestBody);

            using (var stream = new MemoryStream(buffer))
            {
                await blobClient.UploadAsync(stream);
            }

            return new OkObjectResult("OK");
        }
        catch (Exception e)
        {
            log.LogError(e.Message, e);
        }

        return new BadRequestResult();
    }

    private static string GetJsonRandomFileName()
    {
        var fileGuid = Guid.NewGuid();
        var fileName = $"{fileGuid}.json";

        return fileName;
    }
}
