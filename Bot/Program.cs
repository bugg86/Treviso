using System.Configuration;
using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Models.Interfaces;
using oTSPA.Domain.Mongo.Repositories;
using oTSPA.Domain.Mongo.Repositories.Interfaces;
using ConfigurationSection = Microsoft.Extensions.Configuration.ConfigurationSection;

namespace Bot;

public class Program
{
    private DiscordSocketClient? _client;
    private InteractionService _commands = null!;

    public static Task Main(string[] args) => new Program().MainAsync();

    private async Task MainAsync()
    {
        IConfigurationRoot localConfig = LocalBuilder();
        
        var services = ConfigureServices(localConfig);

        _client = services.GetRequiredService<DiscordSocketClient>();
        _commands = services.GetRequiredService<InteractionService>();

        _client.Log += Log;
        _commands.Log += Log;
        _client.Ready += ReadyAsync;


        var token = await File.ReadAllTextAsync("../../../bot_token");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await services.GetRequiredService<CommandHandler>().InitializeAsync();

        await Task.Delay(-1);
    }

    private async Task ReadyAsync()
    {
        await _commands.RegisterCommandsGloballyAsync(true);
    }

    private ServiceProvider ConfigureServices(IConfigurationRoot localConfig)
    {
        var services = new ServiceCollection();

        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<CommandHandler>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.Configure<MongoSettings>(options =>
        {
            options.ConnectionString = localConfig.GetSection("CONNECTION_STRING").Value ?? string.Empty;
            options.DatabaseName = localConfig.GetSection("DATABASE_NAME").Value ?? string.Empty;
        });
        services.AddSingleton<MongoSettings>();

        return services.BuildServiceProvider();
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

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}