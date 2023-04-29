using Discord.Webhook;
using Microsoft.Extensions.Configuration;
using oTSPA.AppService.BotFunctions.Services.Interfaces;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace oTSPA.AppService.BotFunctions.Services;

public class MatchPingService : IMatchPingService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IConfiguration _configuration;

    public MatchPingService(IMatchRepository matchRepository, IConfiguration configuration)
    {
        _matchRepository = matchRepository;
        _configuration = configuration;
    }
    
    public async Task SendPings(string abbreviation)
    {
        List<Match> matches = _matchRepository.FilterBy(x => x.Abbreviation.Equals(abbreviation)).ToList();
        
        var matchPingsWebhook = new DiscordWebhookClient(_configuration.GetSection("MATCH_PINGS_WEBHOOK_URL").Value);
        var refWebhook = new DiscordWebhookClient(_configuration.GetSection("REF_WEBHOOK_URL").Value);
        var streamerWebhook = new DiscordWebhookClient(_configuration.GetSection("STREAMER_WEBHOOK_URL").Value);

        DateTime currentTime = DateTime.Now.ToUniversalTime();
        
        foreach (var match in matches.Where(match => !match.MatchFinished || !match.PingSent))
        {
            DateTime matchTime = DateTime.Parse(string.Concat(match.Date, " ", match.Time));

            // Checks if currentTime is within 15 minutes of matchTime or currentTime is past matchTime.
            // Function triggers every minute so this should be fine to do like this.
            if (currentTime.DayOfYear != matchTime.DayOfYear || matchTime.Minute - currentTime.Minute > 15 ||
                matchTime.Minute - currentTime.Minute <= 0) continue;
            if (match.Referee is null)
            {
                await refWebhook.SendMessageAsync(
                    $"@ Emergency Refs, there is no referee for match id: {match.MatchId}!");
            }
            else
            {
                await refWebhook.SendMessageAsync($"{match.RefereeDiscord}, please get ready for match id {match.MatchId} in about 15 minutes.");
            }

            if (match.Streamer is not null)
            {
                await streamerWebhook.SendMessageAsync($"{match.StreamerDiscord}, " +
                                                       $"{match.Commentator1 ?? string.Empty}, " +
                                                       $"{match.Commentator2 ?? string.Empty} " +
                                                       $"please get ready for match id {match.MatchId} in about 15 minutes. ");
            }
            
            await matchPingsWebhook.SendMessageAsync($"{match.Round} match ``{match.MatchId}`` between ``{match.Team1}``" +
                                                     $" and ``{match.Team2}`` will be starting in about 15 minutes!" +
                                                     $" {match.CaptainDiscord1}, {match.CaptainDiscord2} please get online and prepare" +
                                                     $" for an invite from ``{match.Referee}``!");
        }
    }
}