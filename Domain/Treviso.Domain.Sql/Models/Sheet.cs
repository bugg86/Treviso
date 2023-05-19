using MongoDB.Bson;

namespace Treviso.Domain.Sql.Models;

[BsonCollection("sheets")]
public class Sheet : Document
{
    public ObjectId? TournamentId { get; set; }
    public string Main { get; set; }
    public string Ref { get; set; }
    public string RefType { get; set; }
    public string Pool { get; set; }
    public string Admin { get; set; }
    public ulong User { get; set; }
    public int Version { get; set; }
}