using Microsoft.Extensions.Options;
using Treviso.Domain.Mongo.Models.Interfaces;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories.Interfaces;

namespace Treviso.Domain.Mongo.Repositories;

public class TournamentRepository : Repository<Tournament>, ITournamentRepository
{
    public TournamentRepository(IOptions<MongoSettings> settings) : base(settings)
    {
        
    }
}