using Microsoft.Extensions.Options;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Treviso.Domain.Sql.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(IOptions<MongoSettings> settings) : base(settings)
    {
        
    }
}