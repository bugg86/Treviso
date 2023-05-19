using Microsoft.Extensions.Options;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Treviso.Domain.Sql.Repositories;

public class TournamentRepository : Repository<Tournament>, ITournamentRepository
{
    public TournamentRepository(IOptions<MongoSettings> settings) : base(settings)
    {
        
    }
}