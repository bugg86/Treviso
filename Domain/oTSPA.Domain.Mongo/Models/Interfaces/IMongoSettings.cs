namespace oTSPA.Domain.Mongo.Models.Interfaces;

public interface IMongoSettings
{
    string DatabaseName { get; set; }
    string ConnectionString { get; set; }
}