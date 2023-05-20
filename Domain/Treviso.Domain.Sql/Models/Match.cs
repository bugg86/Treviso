using MongoDB.Bson;

namespace Treviso.Domain.Sql.Models;

public class Match
{
    public Guid Id { get; set; }
    public Guid? TournamentId { get; set; }
    public string MatchId { get; set; }
    public string Round { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Team1 { get; set; }
    public string? CaptainDiscord1 { get; set; }
    public string? Team2 { get; set; }
    public string? CaptainDiscord2 { get; set; }
    public string? Referee { get; set; }
    public string? RefereeDiscord { get; set; }
    public string? Streamer { get; set; }
    public string? Commentator1 { get; set; } //osu! name
    public string? Commentator2 { get; set; } //osu! name
    public bool PingSent { get; set; }
    public bool MatchFinished { get; set; }
    public ulong User { get; set; } //Discord user id of who created the db entry
    public int Version { get; set; } //Model version
    public virtual Tournament Tournament { get; set; }
}