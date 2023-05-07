using Microsoft.Extensions.Options;
using oTSPA.Domain.Mongo.Models;
using oTSPA.Domain.Mongo.Repositories.Interfaces;

namespace oTSPA.Domain.Mongo.Repositories;

public class SheetRepository : Repository<Sheet>, ISheetRepository
{
    public SheetRepository(IOptions<MongoSettings> settings) : base(settings)
    {
        
    }
}