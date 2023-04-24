using System.Text.RegularExpressions;
using Bot.Handlers;
using Discord.Interactions;
using Discord.WebSocket;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Repositories;
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
            List<Tournament> tournaments =
                _tournamentRepository.FilterBy(x => x.Name.Equals(name) && x.Abbreviation.Equals(abbreviation)).ToList();

            if (tournaments.Count != 0)
            {
                await RespondAsync($"A tournament with abbreviation and name: {abbreviation} | {name} was already found, check your parameters and try again.");

                return;
            }
            else
            {
                await _tournamentRepository.InsertOneAsync(newTournament);

                await RespondAsync("Your tournament was successfully added to the database.");
            }
        }
        catch (Exception ex)
        {
            await RespondAsync($"Your tournament could not be added with the following exception: {ex.Message.ToString()}");
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
                    $"Your tournament could not be added with the following exception: {ex.Message.ToString()}");
            }
        }
    }
}