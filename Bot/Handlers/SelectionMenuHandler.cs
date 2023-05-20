using Discord.Interactions;
using Discord.WebSocket;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Bot.Handlers;

public class SelectionMenuHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;
    private readonly ISheetRepository _sheetRepository;
    
    public SelectionMenuHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services, ISheetRepository sheetRepository)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _sheetRepository = sheetRepository;
    }
    public async Task InitializeAsync()
    {
        // await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.SelectMenuExecuted += HandleMenuSelection;
    }

    public async Task HandleMenuSelection(SocketMessageComponent arg)
    {
        switch (arg.Data.CustomId)
        {
            case "tourney-sheet-selection":
                await SheetSelectionMenuHandler(arg);
                break;
        }
    }

    private async Task SheetSelectionMenuHandler(SocketMessageComponent arg)
    {
        string selection = string.Join(", ", arg.Data.Values);
        string[] ids = selection.Split(';');

        // var sheets = _sheetRepository.GetMany(x => x.TournamentId.Equals(new ObjectId(ids[0]))).ToList();
        //
        // if (sheets.Any())
        // {
        //     foreach (Sheet sheet in sheets)
        //     {
        //         _sheetRepository.Remove(sheet);
        //     }
        // }

        // _sheetRepository.Update(filter, updateDefinition);

        await arg.RespondAsync("Sheets added to database.");
    }
}