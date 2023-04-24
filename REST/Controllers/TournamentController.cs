using Microsoft.AspNetCore.Mvc;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace REST.Controllers;

public class TournamentController
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentController(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    [HttpPost("addTournament")]
    public async Task AddTournament()
    {
        
    }
}