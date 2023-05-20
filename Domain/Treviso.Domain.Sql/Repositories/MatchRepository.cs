using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Treviso.Domain.Sql.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(ITrevisoContext trevisoContext) : base(trevisoContext)
    {
        
    }
}