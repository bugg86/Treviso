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
                AddAppServices(services);
                services.AddLogging();
            })
            .ConfigureFunctionsWorkerDefaults()
            .Build();
        
        host.Run();
    }

    private static void AddAppServices(IServiceCollection services)
    {
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IMongoSettings, MongoSettings>();
        services.AddScoped<IMatchPingService, MatchPingService>();
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