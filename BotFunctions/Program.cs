using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Treviso.AppService.BotFunctions.Services;
using Treviso.AppService.BotFunctions.Services.Interfaces;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories;
using Treviso.Domain.Mongo.Repositories.Interfaces;

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
            })
            .ConfigureFunctionsWorkerDefaults()
            .Build();

        host.Run();
    }
    
    private static void AddAppServices(IServiceCollection services, IConfigurationRoot localConfig)
    {
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.Configure<MongoSettings>(options =>
        {
            options.ConnectionString = localConfig.GetSection("CONNECTION_STRING").Value;
            options.DatabaseName = localConfig.GetSection("DATABASE_NAME").Value;
        });
        services.AddSingleton<MongoSettings>();
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