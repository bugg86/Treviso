using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories.Interfaces;

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

    [SlashCommand("addReplace", "add or replace sheets associated with a tournament")]
    public async Task AddSheets(string mainSheet, string adminSheet,  string refSheet, string refType, string poolSheet)
    {
        if (!refType.Equals("hitomiv4") || 
            !refType.Equals("hitomiv5") || 
            !refType.Equals("dioandleo") ||
            !refType.Equals("icedynamix"))
        {
            await RespondAsync("You did not enter a valid ref sheet type. The valid types are: hitomiv4, hitomiv5, dioandleo, and icedynamix. Please enter one of these and try again.");
            return;
        }
        Sheet newSheets = new Sheet
        {
            Main = mainSheet,
            Admin = adminSheet,
            Pool = poolSheet,
            Ref = refSheet,
            RefType = refType,
            User = Context.User.Id,
            Version = 2
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
            //Pass tournament ObjectId and sheet ObjectId through the option custom id
            menuBuilder.AddOption(label: tournament.Abbreviation, tournament.Id.ToString() + ";" + newSheets.Id.ToString(), description: tournament.Name);
        }
        
        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);
        await ReplyAsync("Pick the tournament to add sheets to:", components: builder.Build());
    }
}
