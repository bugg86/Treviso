using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Treviso.Domain.Sql.Contexts;
using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Repositories;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Bot;

public class Program
{
    private DiscordSocketClient? _client;
    private InteractionService _commands = null!;

    public static Task Main(string[] args) => new Program().MainAsync();

    private async Task MainAsync()
    {
        IConfigurationRoot localConfig = LocalBuilder();

        async void ConfigureDelegate(IServiceCollection services)
        {
            ConfigureServices(services, localConfig);
            services.AddDbContextPool<ITrevisoContext, TrevisoContext>(options =>
            {
                options.UseSqlServer(
                    localConfig.GetConnectionString("TREVISO_CONNECTION" ) ?? throw new NullReferenceException("Could not find treviso connection string"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(90));
            }, 1024);

            // services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commands = serviceProvider.GetRequiredService<InteractionService>();

            _client.Log += Log;
            _commands.Log += Log;
            _client.Ready += ReadyAsync;

            var token = localConfig.GetSection("BOT_TOKEN").Value ?? string.Empty;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await serviceProvider.GetRequiredService<SelectionMenuHandler>().InitializeAsync();
            await serviceProvider.GetRequiredService<CommandHandler>().InitializeAsync();
        }

        var host = new HostBuilder()
            .ConfigureAppConfiguration(config => config.AddConfiguration(localConfig))
            .ConfigureServices(ConfigureDelegate);

        await host.RunConsoleAsync();
    }

    private async Task ReadyAsync()
    {
        await _commands.RegisterCommandsGloballyAsync(true);
    }

    private void ConfigureServices(IServiceCollection services, IConfigurationRoot localConfig)
    {
        DiscordSocketConfig config = new()
        {
            UseInteractionSnowflakeDate = false
        };
        services.AddSingleton(config);
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<SelectionMenuHandler>();

        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<ISheetRepository, SheetRepository>();
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