using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Models.Interfaces;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace oTSPA.Domain.Mongo.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(IMongoSettings settings) : base(settings)
    {
        
    }
}