using System.Xml.Schema;
using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

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
    public async Task AddSheets(string mainSheet = "", string adminSheet = "",  string refSheet = "", string refType = "", string poolSheet = "")
    {
        if ((!refType.Equals("hitomiv4") || 
            !refType.Equals("hitomiv5") || 
            !refType.Equals("dioandleo") ||
            !refType.Equals("icedynamix") ||
            !refType.Equals("convex")) && refSheet != "")
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = "You did not enter a valid ref sheet type. " +
                        "The valid types are: hitomiv4, hitomiv5, dioandleo, icedynamix, or convex.",
                Color = Color.Gold
            }.WithCurrentTimestamp().Build());
            return;
        }
        
        if (mainSheet == "" && adminSheet == "" && poolSheet == "" && refSheet == "")
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = "You must include at least one sheet when adding sheets.",
                Color = Color.Gold
            }.WithCurrentTimestamp().Build());
            return;
        }

        Tournament? tourney = await CheckForTournament();

        if (tourney is null) { return; }

        Sheet? sheet = _sheetRepository.GetSingle(x => x.TournamentId.Equals(tourney.Id));

        if (sheet is not null)
        {
            await RespondAsync(embed:new EmbedBuilder()
            {
                Title = "There are already sheets associated with the tournament in this server. " +
                        "Please use the update command, or remove the old sheets before adding new ones.",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }

        Sheet newSheets = new Sheet
        {
            Id = new Guid(),
            TournamentId = tourney.Id,
            Main = mainSheet,
            Admin = adminSheet,
            Pool = poolSheet,
            Ref = refSheet,
            RefType = refType,
            User = Context.User.Id,
            Version = 1
        };
        
        try
        {
            _sheetRepository.Add(newSheets);
            _sheetRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = $"Your sheets could not be added with the following exception: {ex.Message}",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Successfully added sheets to database.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build());
    }
    [SlashCommand("remove", "remove sheets associated with tournament/server")]
    public async Task RemoveSheets()
    {
        Tournament? tourney = await CheckForTournament();

        if (tourney is null) { return; }

        Sheet? sheet = _sheetRepository.GetSingle(x => x.TournamentId.Equals(tourney.Id));

        if (sheet is null)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = "There are no sheets to remove.",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }
        
        try
        {
            _sheetRepository.Remove(sheet);
            _sheetRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = $"Your sheets could not be removed with the following exception: {ex.Message}",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Your sheets were successfully removed.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build());
    }
    [SlashCommand("update", "update existing sheets")]
    public async Task UpdateSheets(string mainSheet = "", string adminSheet = "",  string refSheet = "", string refType = "", string poolSheet = "")
    {
        Tournament? tourney = await CheckForTournament();

        if (tourney is null) { return; }
        
        if ((!refType.Equals("hitomiv4") || 
             !refType.Equals("hitomiv5") || 
             !refType.Equals("dioandleo") ||
             !refType.Equals("icedynamix") ||
             !refType.Equals("convex") ||
             refType.Equals("")) && refSheet != "")
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = "You did not enter a valid ref sheet type. " +
                        "The valid types are: hitomiv4, hitomiv5, dioandleo, icedynamix, or convex.",
                Color = Color.Gold
            }.WithCurrentTimestamp().Build());
            return;
        }

        Sheet? oldSheet = _sheetRepository.GetSingle(x => x.TournamentId.Equals(tourney.Id));

        if (oldSheet is null)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = "There are no sheets to update.",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }

        try
        {
            oldSheet.Main = string.IsNullOrEmpty(mainSheet) ? oldSheet.Main : mainSheet;
            oldSheet.Admin = string.IsNullOrEmpty(adminSheet) ? oldSheet.Admin : adminSheet;
            oldSheet.Pool = string.IsNullOrEmpty(poolSheet) ? oldSheet.Pool : poolSheet;
            oldSheet.Ref = string.IsNullOrEmpty(refSheet) ? oldSheet.Ref : refSheet;
            oldSheet.RefType = string.IsNullOrEmpty(refType) ? oldSheet.RefType : refType;
            oldSheet.User = Context.User.Id;

            _sheetRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = $"Your sheets could not be updated with the following exception: {ex.Message}",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Your sheets were successfully updated.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build());
    }

    private async Task<Tournament?> CheckForTournament()
    {
        Tournament? tourney = _tournamentRepository.GetSingle(x => x.GuildId.Equals(Context.Guild.Id));

        if (tourney is not null) { return tourney; }
        
        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "There is no tournament associated with this server.",
            Color = Color.Red
        }.WithCurrentTimestamp().Build());
        
        return null;
    }
}
