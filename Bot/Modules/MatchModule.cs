using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories.Interfaces;

namespace Bot.Modules;

[Group("match", "commands for managing matches")]
public class MatchModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; } = null!;
    public CommandHandler _handler;
    private readonly IMatchRepository _matchRepository;
    
    public MatchModule(CommandHandler handler, IMatchRepository matchRepository)
    {
        _handler = handler;
        _matchRepository = matchRepository;
    }

    [SlashCommand("add", "command to add a match to database")]
    public async Task AddMatch(string matchId, string abbreviation, string round,
        string? date = null,
        string? time = null, 
        string? team1 = null, 
        string? captainDiscord1 = null, 
        string? team2 = null, 
        string? captainDiscord2 = null, 
        string? referee = null, 
        string? refereeDiscord = null,
        string? streamer = null, 
        string? streamerDiscord = null,
        string? commentator1 = null, 
        string? commentator2 = null)
    {
        var newMatch = new Match
        {
            MatchId = matchId,
            Abbreviation = abbreviation,
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
            StreamerDiscord = streamerDiscord,
            Commentator1 = commentator1,
            Commentator2 = commentator2,
            PingSent = false,
            MatchFinished = false,
            User = Context.User.Id,
            Version = 1
        };

        try
        {
            List<Match> match = _matchRepository.FilterBy(x =>
                x.MatchId.Equals(matchId) && x.Abbreviation!.Equals(abbreviation)).ToList();
            if (match.Count == 1)
            {
                await RespondAsync(embed: MatchCreationExists(matchId, abbreviation));

                return;
            }
            await _matchRepository.InsertOneAsync(newMatch);
            
            await RespondAsync(embed: MatchCreationSuccess());
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: MatchCreationFail(ex));
        }
    }

    [SlashCommand("remove", "command to remove match from database")]
    public async Task RemoveMatch(string matchId, string abbreviation)
    {
        try
        {
            List<Match> match = _matchRepository.FilterBy(x => x.MatchId.Equals(matchId) && x.Abbreviation!.Equals(abbreviation)).ToList();

            if (match.Count == 0)
            {
                await RespondAsync(embed: MatchRemovalNone());
            }
            else
            {
                await _matchRepository.DeleteOneAsync(x => x.MatchId == matchId && x.Abbreviation == abbreviation);
                
                await RespondAsync(embed: MatchRemovalSuccess(matchId, abbreviation));
            }
        }
        catch (Exception ex)
        {
            await RespondAsync(embed: MatchRemovalException(ex));
        }
    }

    [SlashCommand("ping", "command to test match pings")]
    public async Task PingMatch(string matchId, string abbreviation)
    {
        try
        {
            var matchPingChannel = Context.Guild.GetTextChannel(ulong.Parse(await File.ReadAllTextAsync("../../../match-ping-channel-id")));
            var refChannel = Context.Guild.GetTextChannel(ulong.Parse(await File.ReadAllTextAsync("../../../ref-channel-id")));
            var streamerChannel = Context.Guild.GetTextChannel(ulong.Parse(await File.ReadAllTextAsync("../../../streamer-channel-id")));
            
            var match = await _matchRepository.FindOneAsync(x => x.MatchId.Equals(matchId) && x.Abbreviation!.Equals(abbreviation));
            
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
                await streamerChannel.SendMessageAsync($"{match.StreamerDiscord}, " +
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
    private static Embed MatchCreationSuccess()
    {
        return new EmbedBuilder()
        {
            Title = "Your match was successfully added to the database.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build();
    }

    private static Embed MatchCreationExists(string matchId, string abbreviation)
    {
        return new EmbedBuilder()
        {
            Title = $"A match with id: {matchId} already exists for tournament: {abbreviation}.",
            Color = Color.Gold
        }.WithCurrentTimestamp().Build();
    }
    private static Embed MatchCreationFail(Exception ex)
    {
        return new EmbedBuilder()
        {
            Title = $"Your match could not be added with the following exception: {ex.Message}",
            Color = Color.Red
        }.WithCurrentTimestamp().Build();
    }

    private static Embed MatchRemovalSuccess(string matchId, string abbreviation)
    {
        return new EmbedBuilder()
        {
            Title = $"Match ID: {matchId} for tournament: {abbreviation} was successfully removed.",
            Color = Color.Green
        }.WithCurrentTimestamp().Build();
    }
    private static Embed MatchRemovalNone()
    {
        return new EmbedBuilder()
        {
            Title = "Your match could not be found, check your parameters and try again.",
            Color = Color.Gold
        }.WithCurrentTimestamp().Build();
    }
    private static Embed MatchRemovalException(Exception ex)
    {
        Console.WriteLine(ex.Message);
        
        return new EmbedBuilder()
        {
            Title = $"Your match could not be removed with the following exception: {ex.Message}",
            Color = Color.DarkRed
        }.WithCurrentTimestamp().Build();
    }
}