using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

[assembly: FunctionsStartup(typeof(DeliveryFunctionApp.Startup))]
namespace DeliveryFunctionApp;

public class Startup : FunctionsStartup
{
    private const string DevelopmentEnvironment = "Development";
    private const string LocalSettingsJsonFileName = "local.settings.json";
    private const string SecretsJsonFileName = "secrets.json";
    private const string KeyVaultURI = "KeyVaultURI";

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        AddLocalConfigurations(builder);
        AddAzureKeyVaultSecrets(builder);
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

    public override void Configure(IFunctionsHostBuilder builder)
    {
    }
}
