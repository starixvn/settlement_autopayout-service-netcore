namespace DirectPayout.Early.Models;

public class Match
{
    public int MatchId { get; set; }

    public int LeagueId { get; set; }

    public List<ResultDetail> ResultDetails { get; set; } = [];
}