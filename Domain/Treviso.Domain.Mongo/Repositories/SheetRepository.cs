using Microsoft.Extensions.Options;
using Treviso.Domain.Mongo.Models;
using Treviso.Domain.Mongo.Repositories.Interfaces;

namespace Treviso.Domain.Mongo.Repositories;

public class SheetRepository : Repository<Sheet>, ISheetRepository
{
    public SheetRepository(IOptions<MongoSettings> settings) : base(settings)
    {
        
    }
}