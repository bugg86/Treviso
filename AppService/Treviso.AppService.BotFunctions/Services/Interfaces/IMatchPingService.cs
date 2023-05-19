namespace Treviso.AppService.BotFunctions.Services.Interfaces;

public interface IMatchPingService
{
    public Task SendPings(string tournamentId);
}