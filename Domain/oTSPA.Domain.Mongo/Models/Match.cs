namespace oTSPA.Domain.Mongo.Models;

[BsonCollection("matches")]
public class Match : Document
{
    public string MatchId { get; set; }
    public string Abbreviation { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Team1 { get; set; }
    public string? CaptainDiscord1 { get; set; }
    public string? Team2 { get; set; }
    public string? CaptainDiscord2 { get; set; }
    public string? Referee { get; set; }
    public string? Streamer { get; set; }
    public string? Commentator1 { get; set; }
    public string? Commentator2 { get; set; }
    public ulong User { get; set; } //Discord user id of who created the db entry
    public int Version { get; set; } //Model version
}