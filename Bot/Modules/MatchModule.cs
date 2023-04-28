using Bot.Handlers;
using Discord;
using Discord.Interactions;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

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
    public async Task AddMatch(string matchId, string abbreviation,
        string? date = null,
        string? time = null, 
        string? team1 = null, 
        string? captainDiscord1 = null, 
        string? team2 = null, 
        string? captainDiscord2 = null, 
        string? referee = null, 
        string? streamer = null, 
        string? commentator1 = null, 
        string? commentator2 = null)
    {
        var newMatch = new Match
        {
            MatchId = matchId,
            Abbreviation = abbreviation,
            Date = date,
            Time = time,
            Team1 = team1,
            CaptainDiscord1 = captainDiscord1,
            Team2 = team2,
            CaptainDiscord2 = captainDiscord2,
            Referee = referee,
            Streamer = streamer,
            Commentator1 = commentator1,
            Commentator2 = commentator2,
            User = Context.User.Id,
            Version = 1
        };

        try
        {
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
            List<Match> match = _matchRepository.FilterBy(x => x.MatchId.Equals(matchId) && x.Abbreviation.Equals(abbreviation)).ToList();

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

    private static Embed MatchCreationSuccess()
    {
        return new EmbedBuilder()
        {
            Title = "Your match was successfully added to the database.",
            Color = Color.Green
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