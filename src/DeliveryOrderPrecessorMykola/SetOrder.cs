using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DeliveryCore.Models;

namespace DeliveryOrderPrecessorMykola;

public static class SetOrder
{
    [FunctionName("DeliveryOrderPrecessorMykola")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [CosmosDB(
                databaseName: "azurecosmosdbaccountmykola",
                collectionName: "DeliveryOrders",
                CreateIfNotExists = true,
                PartitionKey = "/id",
                ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,
        ILogger log)
    {
        string requestBody = new StreamReader(req.Body).ReadToEnd();
        log.LogInformation($"Processing request: {requestBody}");
        dynamic data = JsonConvert.DeserializeObject<DeliveryOrder>(requestBody);
        document = data;
        return new OkObjectResult("OK");
    }
}
