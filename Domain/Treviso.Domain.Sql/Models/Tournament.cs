namespace Treviso.Domain.Sql.Models;

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
    public int TeamSize { get; set; }
    public string Vs { get; set; } //Ex: 3v3
    public ulong User { get; set; } //Discord user id of who created the db entry
    public int Version { get; set; } //Model version
}