using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Treviso.AppService.BotFunctions.Services.Interfaces;
using TimerInfo = Treviso.Domain.Data.Models.TimerInfo;

namespace BotFunctions.Functions;

public class MatchPings
{
    private readonly IConfiguration _configuration;
    private readonly IMatchPingService _matchPingService;

    public MatchPings(IConfiguration configuration, IMatchPingService matchPingService)
    {
        _configuration = configuration;
        _matchPingService = matchPingService;
    }
    [Function("MatchPings")]
    public async Task Run([TimerTrigger("0 0/1 * * * *", RunOnStartup = true)] TimerInfo myTimer, FunctionContext context)
    {
        await _matchPingService.SendPings("CDC2");
    }
}