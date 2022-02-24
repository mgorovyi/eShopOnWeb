using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Microsoft.eShopWeb.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) => {
                var env = hostingContext.HostingEnvironment;
                if (env.IsDevelopment())
                {
                    var assembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (assembly != null)
                    {
                        config.AddUserSecrets(assembly, true);
                    }
                }

                // Build the current set of configuration to load values from
                // JSON files and environment variables, including VaultName.
                var builtConfig = config.Build();
                var keyVaultUri = builtConfig["KeyVaultURI"];
                if (!string.IsNullOrWhiteSpace(keyVaultUri))
                {
                    Uri vaultUri = new Uri(keyVaultUri);
                    config.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
                }

            }).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var catalogContext = services.GetRequiredService<CatalogContext>();
                await CatalogContextSeed.SeedAsync(catalogContext, loggerFactory);

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
