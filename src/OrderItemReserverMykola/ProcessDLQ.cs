using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace OrderItemReserverMykola;

public class ProcessDLQ
{
    private readonly EmailSenderLogicAppClient _senderLogicAppClient;

    public ProcessDLQ(EmailSenderLogicAppClient senderLogicAppClient)
    {
        _senderLogicAppClient = senderLogicAppClient;
    }

    [ServiceBusAccount("ServiceBusConnectionString")]
    [FunctionName("ProcessDLQ")]
    public async Task Run([ServiceBusTrigger("ordercompleted/$DeadLetterQueue", AutoCompleteMessages = true)] string message, 
                            ILogger log)
    {
        log.LogWarning($"ServiceBus Dead Letter Queue trigger function processed message: {message}");
        await _senderLogicAppClient.SendEmailAsync(message);
    }
}
