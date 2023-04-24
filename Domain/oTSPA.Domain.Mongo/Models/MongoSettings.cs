using oTSPA.Domain.Mongo.Models.Interfaces;

namespace oTSPA.Domain.Mongo.Models;

public class MongoSettings : IMongoSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "staff-tool";
}