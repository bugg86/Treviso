using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using oTSPA.Domain.Mongo.Repositories;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace Bot;

public class Program
{
    private DiscordSocketClient? _client;
    private InteractionService _commands;
    
    public static Task Main(string[] args) => new Program().MainAsync();

    private async Task MainAsync()
    {
        var services = ConfigureServices();
        
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

    private ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<CommandHandler>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();

        return services.BuildServiceProvider();
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}