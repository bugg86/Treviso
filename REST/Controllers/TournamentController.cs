using oTSPA.Domain.Mongo.Models.Interfaces;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace REST.Controllers;

public class TournamentController
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentController(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }
}