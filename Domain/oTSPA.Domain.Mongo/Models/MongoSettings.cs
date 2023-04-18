namespace oTSPA.Domain.Mongo.Models;

public class MongoSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}