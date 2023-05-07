using Bot.Handlers;
using Discord;
using Discord.Interactions;
using MongoDB.Bson;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace Bot.Modules;

[Group("sheet", "commands related to managing sheets")]
public class SheetModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; } = null!;
    public CommandHandler _handler;
    private readonly ISheetRepository _sheetRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public SheetModule(CommandHandler handler, ISheetRepository sheetRepository, ITournamentRepository tournamentRepository)
    {
        _handler = handler;
        _sheetRepository = sheetRepository;
        _tournamentRepository = tournamentRepository;
    }

    [SlashCommand("add", "add sheets associated with a tournament")]
    public async Task AddSheets(string mainSheet, string? adminSheet = null,  string? refSheet = null, string? poolSheet = null)
    {
        Sheet newSheets = new Sheet
        {
            Main = mainSheet,
            Admin = adminSheet,
            Pool = poolSheet,
            Ref = refSheet
        };

        await _sheetRepository.InsertOneAsync(newSheets);

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select a tourney")
            .WithCustomId("tourney-sheet-selection")
            .WithMinValues(1)
            .WithMaxValues(1);

        IList<Tournament> tournaments = _tournamentRepository.FilterBy(x => !string.IsNullOrEmpty(x.Abbreviation)).ToList();

        if (!tournaments.Any())
        {
            await RespondAsync("There are no tournaments to add sheets to.");
            
            return;
        }

        foreach (Tournament tournament in tournaments)
        {
            //Pass tournament object and sheet objects through the option custom id
            menuBuilder.AddOption(label: tournament.Abbreviation, tournament.Id.ToString() + ";" + newSheets.Id.ToString(), description: tournament.Name);
        }
        
        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
        await ReplyAsync("Pick the tournament to add sheets to:", components: builder.Build());
    }
}

