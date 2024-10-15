namespace DirectPayout.Early.Models;

public class Match
{
    public int MatchId { get; set; }

    public int LeagueId { get; set; }

    public int ResourceId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }

    public DateTime LastModifyDate { get; set; }

    public int WaitingTime { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Match match
               && match.MatchId == MatchId
               && match.ResourceId == ResourceId;
    }

    public override int GetHashCode() => MatchId;
}