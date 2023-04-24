using Bot.Handlers;
using Discord.Interactions;
using Discord.WebSocket;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace Bot.Modules;

[Group("tournament", "commands for managing tournament")]
public class TournamentModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }
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
            Version = 1,
            User = Context.User.Id
        };

        try
        {
            await _tournamentRepository.InsertOneAsync(newTournament);

            await RespondAsync("Your tournament was successfully added to the database.");
        }
        catch (Exception ex)
        {
            await RespondAsync($"Your tournament could not be added with the following exception: {ex.Message.ToString()}");
        }
    }
}