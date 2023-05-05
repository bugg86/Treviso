using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using oTSPA.AppService.BotFunctions.Services;
using oTSPA.AppService.BotFunctions.Services.Interfaces;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Models.Interfaces;
using oTSPA.Domain.Mongo.Repositories;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

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