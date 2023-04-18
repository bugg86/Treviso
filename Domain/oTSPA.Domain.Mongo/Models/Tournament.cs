using MongoDB.Bson;

namespace oTSPA.Domain.Mongo.Models;

[BsonCollection("tournaments")]
public class Tournament : Document
{
    public string Abbreviation { get; set; }
    public bool Badged { get; set; }
    public bool Bws { get; set; }
    public decimal Iteration { get; set; }
    public string Name { get; set; }
    public string RangeLower { get; set; }
    public string RangeUpper { get; set; }
    public int Version { get; set; }
}