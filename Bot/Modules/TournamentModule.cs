using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Bot.Modules;

[Group("tournament", "commands for managing tournament")]
public class TournamentModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; } = null!;
    public CommandHandler _handler;
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentModule(CommandHandler handler, ITournamentRepository tournamentRepository)
    {
        _handler = handler;
        _tournamentRepository = tournamentRepository;
    }

    [SlashCommand("add", "command to add tournament to database")]
    public async Task AddTournament(string abbreviation, string name, bool bws, decimal iteration, int teamSize, string vs, bool badged, string rangeLower, string rangeUpper)
    {
        var newTournament = new Tournament
        {
            Abbreviation = abbreviation,
            Name = name,
            Iteration = iteration,
            Bws = bws,
            Badged = badged,
            TeamSize = teamSize,
            Vs = vs,
            RangeLower = rangeLower,
            RangeUpper = rangeUpper,
            User = Context.User.Id,
            Version = 1
        };

        try
        {
            List<Tournament> tournaments =
                _tournamentRepository.FilterBy(x => x.Abbreviation.Equals(abbreviation)).ToList();

            if (tournaments.Count != 0)
            {
                await RespondAsync(embed: TournamentCreationWarning(tournaments, abbreviation));
                
                await _tournamentRepository.InsertOneAsync(newTournament);
            }
            else
            {
                await _tournamentRepository.InsertOneAsync(newTournament);

                await RespondAsync(embed: TournamentCreationSuccess());
            }
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: TournamentCreationFail(ex));
        }
    }

    [SlashCommand("remove", "removes a tournament from the database")]
    public async Task RemoveTournament(string abbreviation, string name)
    {
        List<Tournament> tournaments = _tournamentRepository.FilterBy(x => x.Name.Equals(name) && x.Abbreviation.Equals(abbreviation)).ToList();

        if (tournaments.Count == 0)
        {
            await RespondAsync("There were no tournaments found with that abbreviation and name. Check your parameters and try again.");
        }
        else
        {
            try
            {
                await _tournamentRepository.DeleteOneAsync(x => x.Id == tournaments.ElementAt(0).Id);

                await RespondAsync($"Tournament {abbreviation} | {name} was successfully removed.");
            }
            catch (Exception ex)
            {
                await RespondAsync(
                    $"Your tournament could not be added with the following exception: {ex.Message}");
            }
        }
    }
    private static Embed TournamentCreationSuccess()
    {
        return new EmbedBuilder()
        {
            Title = "Your tournament was successfully added to the database.", 
            Color = Color.Green
        }.WithCurrentTimestamp().Build();
    }
    private static Embed TournamentCreationWarning(IReadOnlyCollection<Tournament> tournaments, string abbreviation)
    {
        var embed = new EmbedBuilder()
        {
            Description = "Your tournament was added but others were found with this abbreviation.",
            Color = Color.Gold
        }.WithCurrentTimestamp();
        
        for (var i = 0; i < tournaments.Count; i++)
        {
            embed.AddField($"Tournaments found with abbreviation: {abbreviation}", $"**{(i+1).ToString()}:** {tournaments.ElementAt(i).Name}", true);
            if (i != 0)
            {
                embed.Fields.ElementAt(0).Value = string.Concat(embed.Fields.ElementAt(0).Value, $"\n, **{(i + 1).ToString()}:** {tournaments.ElementAt(i).Name}");
            }
            if (embed.Fields.Count > 5 && embed.Fields.Count != tournaments.Count)
            {
                embed.AddField("Too many tournaments: ", $" {tournaments.Count - embed.Fields.Count} more tournaments were found...", true);
            }
        }
        
        return embed.Build();
    }
    private static Embed TournamentCreationFail(Exception ex)
    {
        return new EmbedBuilder()
        {
            Title = $"Your tournament could not be added with the following exception: {ex.Message}",
            Color = Color.Red
        }.WithCurrentTimestamp().Build();
    }
}