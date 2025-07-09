namespace AutoPayout.Sportsbook.Models;

public class Score
{
    public int MatchId { get; set; }

    public int SourceId { get; set; }

    public int TypeId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }
}
