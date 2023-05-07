using MongoDB.Bson;

namespace oTSPA.Domain.Mongo.Models;

[BsonCollection("sheets")]
public class Sheet : Document
{
    public ObjectId? TournamentId { get; set; }
    public string? Main { get; set; }
    public string? Ref { get; set; }
    public string? Pool { get; set; }
    public string? Admin { get; set; }
}