using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

[assembly: FunctionsStartup(typeof(OrderItemReserverMykola.Startup))]
namespace OrderItemReserverMykola;
public class Startup : FunctionsStartup
{

    private const string DevelopmentEnvironment = "Development";
    private const string LocalSettingsJsonFileName = "local.settings.json";
    private const string SecretsJsonFileName = "secrets.json";
    private const string KeyVaultURI = "KeyVaultURI";

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;
        //Use Poly retry pattern with named httpclient
        services.AddHttpClient<EmailSenderLogicAppClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {

    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(10));
    }

    void AddLocalConfigurations(IFunctionsConfigurationBuilder builder)
    {
        var environment = builder.GetContext().EnvironmentName;
        if (environment == DevelopmentEnvironment)
        {
            builder.ConfigurationBuilder.AddJsonFile(LocalSettingsJsonFileName, true);
            builder.ConfigurationBuilder.AddUserSecrets(SecretsJsonFileName, true);
        }
    }

    void AddAzureKeyVaultSecrets(IFunctionsConfigurationBuilder builder)
    {
        var builtConfig = builder.ConfigurationBuilder.Build();
        var keyVaultUri = builtConfig[KeyVaultURI];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            Uri vaultUri = new Uri(keyVaultUri);
            builder.ConfigurationBuilder.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
        }
    }
}
