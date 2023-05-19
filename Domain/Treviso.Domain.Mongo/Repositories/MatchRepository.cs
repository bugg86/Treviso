using Microsoft.Extensions.Options;
using Treviso.Domain.Mongo.Models.Interfaces;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories.Interfaces;

namespace Treviso.Domain.Mongo.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(IOptions<MongoSettings> settings) : base(settings)
    {
        
    }
}