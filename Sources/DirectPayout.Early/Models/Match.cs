namespace DirectPayout.Early.Models;

public class Match
{
    public int MatchId { get; set; }

    public int ResourceId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Match match
               && match.MatchId == MatchId;
    }

    public override int GetHashCode() => MatchId;
}