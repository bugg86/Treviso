using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Treviso.Domain.Sql.Repositories;

public class TournamentRepository : Repository<Tournament>, ITournamentRepository
{
    public TournamentRepository(ITrevisoContext trevisoContext) : base(trevisoContext)
    {
        
    }
}