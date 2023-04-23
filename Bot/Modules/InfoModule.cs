using Bot.Handlers;
using Discord.Commands;
using Discord.Interactions;

namespace Bot.Modules;

public class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }
    public CommandHandler _handler;

    public InfoModule(CommandHandler handler)
    {
        _handler = handler;
    }

    [SlashCommand("test", "test description")]
    public async Task TestCommand(string testField)
    {
        await RespondAsync($"here's what you entered: {testField}");
    }
}