using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Treviso.AppService.BotFunctions.Services;
using Treviso.AppService.BotFunctions.Services.Interfaces;
using Treviso.Domain.Sql.Contexts;
using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace BotFunctions;

public class Program
{
    public static void Main()
    {
        IConfigurationRoot localConfig = LocalBuilder();

        var host = new HostBuilder()
            .ConfigureAppConfiguration(config => config.AddConfiguration(localConfig))
            .ConfigureServices(services =>
            {
                AddAppServices(services, localConfig);
                services.AddLogging();
                services.AddDbContextPool<ITrevisoContext, TrevisoContext>(options =>
                {
                    options.UseSqlServer(
                        localConfig.GetConnectionString("TREVISO_CONNECTION" ) ?? throw new NullReferenceException("Could not find treviso connection string"),
                        sqlServerOptions => sqlServerOptions.CommandTimeout(90));
                }, 1024);
            })
            .ConfigureFunctionsWorkerDefaults()
            .Build();

        host.Run();
    }
    
    private static void AddAppServices(IServiceCollection services, IConfigurationRoot localConfig)
    {
        services.AddScoped<IMatchRepository, MatchRepository>();

        services.AddScoped<IMatchPingService, MatchPingService>();
        services.AddSingleton<DiscordSocketClient>();

        services.AddSingleton<GoogleSheetsService>();
        services.AddScoped<IGoogleSheetsController, GoogleSheetsController>();
    }
    private static IConfigurationRoot LocalBuilder()
    {
        IConfigurationBuilder localConfigBuilder = new ConfigurationBuilder().AddEnvironmentVariables();
        IConfigurationRoot localConfig = localConfigBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true).Build();

        // if (localConfig.GetConnectionString("APP_CONFIG") == null) { return localConfig; }

        return new ConfigurationBuilder()
            .AddConfiguration(localConfig)
            .Build();
    }
}