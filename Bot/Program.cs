using Discord;
using Discord.WebSocket;

public class Program
{
    private DiscordSocketClient _client;
    
    public static Task Main(string[] args) => new Program().MainAsync();
    
    private async Task MainAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += Log;

        var token = File.ReadAllText("../../../bot_token");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}