using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Bot.Modules;

[Group("match", "commands for managing matches")]
public class MatchModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; } = null!;
    public CommandHandler _handler;
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IConfiguration _configuration;

    public MatchModule(CommandHandler handler, IMatchRepository matchRepository, ITournamentRepository tournamentRepository, IConfiguration configuration)
    {
        _handler = handler;
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
        _configuration = configuration;
    }

    [SlashCommand("add", "command to add a match to database")]
    public async Task AddMatch
    (
        string matchId, string round,
        string date = "",
        string time = "", 
        string team1 = "", 
        string captainDiscord1 = "", 
        string team2 = "", 
        string captainDiscord2 = "", 
        string referee = "", 
        string refereeDiscord = "",
        string streamer = "",
        string commentator1 = "", 
        string commentator2 = ""
    )
    {
        Tournament? tourney = await CheckForTournament();

        if (tourney is null) { return; }
        
        Match newMatch = new Match
        {
            Id = new Guid(),
            TournamentId = tourney.Id,
            MatchId = matchId,
            Round = round,
            Date = date,
            Time = time,
            Team1 = team1,
            CaptainDiscord1 = captainDiscord1,
            Team2 = team2,
            CaptainDiscord2 = captainDiscord2,
            Referee = referee,
            RefereeDiscord = refereeDiscord,
            Streamer = streamer,
            Commentator1 = commentator1,
            Commentator2 = commentator2,
            PingSent = false,
            MatchFinished = false,
            User = Context.User.Id,
            Version = 1
        };

        Match? tempMatch = _matchRepository.GetSingle(x => x.MatchId.Equals(matchId));

        if (tempMatch is not null)
        {
            new EmbedBuilder()
            {
                Title = $"A match with id of {matchId} already exists.",
                Color = Color.Red
            }.WithCurrentTimestamp().Build();
            return;
        }

        try
        {
            _matchRepository.Add(newMatch);
            _matchRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = $"Your match could not be added with the following exception: {ex.Message}",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }
        
        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Your match was successfully added to the database.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build());
    }

    [SlashCommand("remove", "command to remove match from database")]
    public async Task RemoveMatch(string matchId)
    {
        Tournament? tourney = await CheckForTournament();

        if (tourney is null) { return; }
        
        Match? match = _matchRepository.GetSingle(x => x.MatchId.Equals(matchId));

        if (match is null)
        {
            new EmbedBuilder()
            {
                Title = $"A match with id of {matchId} does not exist.",
                Color = Color.Red
            }.WithCurrentTimestamp().Build();
            return;
        }
        
        try
        {
            _matchRepository.Remove(match);
            _matchRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = $"Your match could not be removed with the following exception: {ex.Message}",
                Color = Color.DarkRed
            }.WithCurrentTimestamp().Build());
            return;
        }
        
        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = $"Match ID: {matchId} for tournament: {tourney.Abbreviation} was successfully removed.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build());
    }
    [SlashCommand("update", "update match data")]
    public async Task UpdateMatch
    (
        string matchId,
        string? round = null,
        string? date = null,
        string? time = null, 
        string? team1 = null, 
        string? captainDiscord1 = null, 
        string? team2 = null, 
        string? captainDiscord2 = null, 
        string? referee = null, 
        string? refereeDiscord = null,
        string? streamer = null,
        string? commentator1 = null, 
        string? commentator2 = null,
        bool? pingSent = null,
        bool? matchFinished = null
    )
    {
        Tournament? tourney = await CheckForTournament();

        if (tourney is null) { return; }
        
        Match? oldMatch = _matchRepository.GetSingle(x => x.MatchId.Equals(matchId));

        if (oldMatch is null)
        {
            new EmbedBuilder()
            {
                Title = $"A match with id of {matchId} does not exist.",
                Color = Color.Red
            }.WithCurrentTimestamp().Build();
            return;
        }

        try
        {
            oldMatch.Round = round ?? oldMatch.Round;
            oldMatch.Date = date ?? oldMatch.Date;
            oldMatch.Time = time ?? oldMatch.Time;
            oldMatch.Team1 = team1 ?? oldMatch.Team1;
            oldMatch.CaptainDiscord1 = captainDiscord1 ?? oldMatch.CaptainDiscord1;
            oldMatch.Team2 = team2 ?? oldMatch.Team2;
            oldMatch.CaptainDiscord2 = captainDiscord2 ?? oldMatch.CaptainDiscord2;
            oldMatch.Referee = referee ?? oldMatch.Referee;
            oldMatch.RefereeDiscord = refereeDiscord ?? oldMatch.RefereeDiscord;
            oldMatch.Streamer = streamer ?? oldMatch.Streamer;
            oldMatch.Commentator1 = commentator1 ?? oldMatch.Commentator1;
            oldMatch.Commentator2 = commentator2 ?? oldMatch.Commentator2;
            oldMatch.PingSent = pingSent ?? oldMatch.PingSent;
            oldMatch.MatchFinished = matchFinished ?? oldMatch.MatchFinished;

            _matchRepository.SaveChanges();
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: new EmbedBuilder()
            {
                Title = $"Your match could not be updated with the following exception: {ex.Message}",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }
        
        await RespondAsync(embed: new EmbedBuilder()
        {
            Title = "Your match was successfully updated.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build());
    }
    [SlashCommand("ping", "command to test match pings")]
    public async Task PingMatch(string matchId, string abbreviation)
    {
        try
        {
            var matchPingChannel = Context.Guild.GetTextChannel(ulong.Parse(_configuration.GetSection("MATCH_PINGS_CHANNEL_ID").Value ?? string.Empty));
            var refChannel = Context.Guild.GetTextChannel(ulong.Parse(_configuration.GetSection("REF_CHANNEL_ID").Value ?? string.Empty));
            var streamerChannel = Context.Guild.GetTextChannel(ulong.Parse(_configuration.GetSection("STREAMER_CHANNEL_ID").Value ?? string.Empty));
            
            var match = _matchRepository.GetSingle(x => x.MatchId.Equals(matchId));

            if (match is null)
            {
                await RespondAsync("No match found");

                return;
            }
            
            if (match.Referee is not null)
            {
                await refChannel.SendMessageAsync($"{match.RefereeDiscord}, please get ready for match id {match.MatchId} in about 15 minutes.");  
            }
            else
            {
                await refChannel.SendMessageAsync(
                    $"@ Emergency Refs, there is no referee for match id: {match.MatchId}!");
            }

            if (match.Streamer is not null)
            {
                await streamerChannel.SendMessageAsync($"{match.Streamer}, " +
                                                       $"{match.Commentator1 ?? string.Empty}, " +
                                                       $"{match.Commentator2 ?? string.Empty} " +
                                                       $"please get ready for match id {match.MatchId} in about 15 minutes. ");
            }
            
            await matchPingChannel.SendMessageAsync($"{match.Round} match ``{match.MatchId}`` between ``{match.Team1}``" +
                                                    $" and ``{match.Team2}`` will be starting in about 15 minutes!" +
                                                    $" {match.CaptainDiscord1}, {match.CaptainDiscord2} please get online and prepare" +
                                                    $" for an invite from ``{match.Referee}``!");

            await RespondAsync("Ping messages sent!");
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: MatchPingFail());
        }
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
    private static string CreateMatchPing(Match match)
    {
        return $"{match.Round} match ``{match.MatchId}`` between ``{match.Team1}``" +
               $" and ``{match.Team2}`` will be starting in about 15 minutes!" +
               $" {match.CaptainDiscord1}, {match.CaptainDiscord2} please get online and prepare" +
               $" for an invite from ``{match.Referee}``!";
    }
    private static Embed MatchPingFail()
    {
        return new EmbedBuilder()
        {
            Title = "Could not create ping, match likely does not exist.",
            Color = Color.Red
        }.WithCurrentTimestamp().Build();
    }
}