using Bot.Handlers;
using Discord.Commands;
using Discord.Interactions;

namespace Bot.Modules;

public class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; } = null!;
    public CommandHandler _handler;

    public InfoModule(CommandHandler handler)
    {
        _handler = handler;
    }
}