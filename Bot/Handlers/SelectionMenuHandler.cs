using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Driver;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories.Interfaces;

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

        var sheets = _sheetRepository.FilterBy(x => x.TournamentId.Equals(new ObjectId(ids[0]))).ToList();

        if (sheets.Any())
        {
            foreach (Sheet sheet in sheets)
            {
                await _sheetRepository.DeleteByIdAsync(sheet.Id.ToString());
            }
        }
        
        var filter = Builders<Sheet>.Filter.Eq(x => x.Id, new ObjectId(ids[1]));
        var updateDefinition = Builders<Sheet>.Update.Set(x => x.TournamentId, new ObjectId(ids[0]));

        await _sheetRepository.UpdateOneAsync(filter, updateDefinition);

        await arg.RespondAsync("Sheets added to database.");
    }
}