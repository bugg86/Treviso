namespace Treviso.Domain.Sql.Models;

public class Tournament
{
    public Guid Id { get; set; }
    public ulong GuildId { get; set; }
    public string Abbreviation { get; set; }
    public bool Badged { get; set; }
    public bool Bws { get; set; }
    public string Name { get; set; }
    public string RangeLower { get; set; }
    public string RangeUpper { get; set; }
    public int TeamSize { get; set; }
    public string Vs { get; set; } //Ex: 3v3
    public ulong User { get; set; } //Discord user id of who created the db entry
    public int Version { get; set; } //Model version
    public virtual Sheet Sheet { get; set; }
    public virtual ICollection<Match> Matches { get; set; }
}