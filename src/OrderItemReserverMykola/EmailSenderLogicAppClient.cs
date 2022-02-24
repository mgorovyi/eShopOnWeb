using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OrderItemReserverMykola;

public class EmailSenderLogicAppClient
{
    //https://medium.com/@gmobrice/triggering-azure-logic-apps-with-http-requests-468c75a4d5f6

    private readonly HttpClient _httpClient;
    private static string logicAppUri = @"https://prod-37.eastus2.logic.azure.com:443/workflows/f087ad9a13524d879f2d315797a7c16f/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=7ZM3db1JjruN8Vc02z0wYVyCc0k_CP6c9kbOxuxsvxM";
    public EmailSenderLogicAppClient(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> SendEmailAsync(string productsMessage)
    {
        //request logic app to send email
        var body = new { products = productsMessage };
        var json = JsonSerializer.Serialize(body);

        var content = new StringContent(json);
        content.Headers.ContentType.CharSet = string.Empty;
        content.Headers.ContentType.MediaType = "application/json";

        var response = await _httpClient.PostAsync(logicAppUri, content);

        return response;
    }
}
