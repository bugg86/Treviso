using oTSPA.Domain.Mongo.Models.Interfaces;

namespace oTSPA.Domain.Mongo.Models;

public class MongoSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}