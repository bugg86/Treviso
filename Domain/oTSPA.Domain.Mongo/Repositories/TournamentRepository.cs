using MongoDB.Driver;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Models.Interfaces;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace oTSPA.Domain.Mongo.Repositories;

public class TournamentRepository : Repository<Tournament>, ITournamentRepository
{
    public TournamentRepository(IMongoSettings settings) : base(settings)
    {
        
    }
}