namespace DirectPayout.Early.Models;

public class Match
{
    public int MatchId { get; set; }

    public int LeagueId { get; set; }

    public short SportType { get; set; }

    public short ChildMatchType { get; set; }

    public List<ResultDetail> ResultDetails { get; set; } = [];

    public override bool Equals(object obj)
    {
        return obj is Match match
               && match.MatchId == MatchId;
    }

    public override int GetHashCode() => MatchId;


    public override string ToString() => $"Match({MatchId}) has {string.Join(",", ResultDetails.Select(x => x.ToString()))}";
}