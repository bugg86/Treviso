namespace Treviso.Domain.Sql.Models;

public class Sheet
{
    public Guid Id { get; set; }
    public Guid TournamentId { get; set; }
    public string Main { get; set; }
    public string Ref { get; set; }
    public string RefType { get; set; }
    public string Pool { get; set; }
    public string Admin { get; set; }
    public ulong User { get; set; }
    public int Version { get; set; }
    
    public virtual Tournament Tournament { get; set; }
}