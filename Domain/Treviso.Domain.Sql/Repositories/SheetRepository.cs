using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Models;
using Treviso.Domain.Sql.Repositories.Interfaces;

namespace Treviso.Domain.Sql.Repositories;

public class SheetRepository : Repository<Sheet>, ISheetRepository
{
    public SheetRepository(ITrevisoContext trevisoContext) : base(trevisoContext)
    {
        
    }
}