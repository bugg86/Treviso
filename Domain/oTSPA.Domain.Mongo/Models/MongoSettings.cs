using oTSPA.Domain.Mongo.Models.Interfaces;

namespace oTSPA.Domain.Mongo.Models;

public class MongoSettings : IMongoSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}