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
    public async Task AddTournament(string abbreviation, string name, bool bws, int teamSize, string vs, bool badged, string rangeLower, string rangeUpper)
    {
        var newTournament = new Tournament
        {
            Id = new Guid(),
            GuildId = Context.Guild.Id,
            Abbreviation = abbreviation,
            Name = name,
            Bws = bws,
            Badged = badged,
            TeamSize = teamSize,
            Vs = vs,
            RangeLower = rangeLower,
            RangeUpper = rangeUpper,
            User = Context.User.Id,
            Version = 1
        };

        Tournament? tourney = _tournamentRepository.GetSingle(x => x.GuildId.Equals(Context.Guild.Id));

        if (tourney is null)
        {
            _tournamentRepository.Add(newTournament);
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = "Your tournament was successfully added to the database.", 
                Color = Color.Green
            }.WithCurrentTimestamp().Build());
            
            return;
        }

        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Your tournament could not be added because there is already one associated with the discord.",
            Color = Color.Red
        }.WithCurrentTimestamp().Build());
    }

    [SlashCommand("remove", "removes a tournament from the database")]
    public async Task RemoveTournament(string abbreviation, string name)
    {
        Tournament? tournament = _tournamentRepository.GetSingle(x => x.GuildId.Equals(Context.Guild.Id));

        if (tournament is null)
        {
            await RespondAsync("There were no tournaments found associated with this server.");
            
            return;
        }
        try
        {
            _tournamentRepository.Remove(tournament);

            await RespondAsync($"Tournament {abbreviation} | {name} was successfully removed.");
        }
        catch (Exception ex)
        {
            await RespondAsync(
                $"Your tournament could not be added with the following exception: {ex.Message}");
        }
    }
}