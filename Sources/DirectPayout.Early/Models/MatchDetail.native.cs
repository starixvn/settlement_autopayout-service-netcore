namespace DirectPayout.Early.Models;
public class MatchDetail
{
    public int MatchId { get; set; }

    public string MatchCode { get; set; }

    public int LeagueId { get; set; }

    public string KickOffTime { get; set; }

    public int HomeId { get; set; }

    public int AwayId { get; set; }

    public int PlayTime { get; set; }

    public int BestOfMap { get; set; }

    public int? HalfTimeHomeScore { get; set; }

    public int? HalfTimeAwayScore { get; set; }

    public int? FinalHomeScore { get; set; }

    public int? FinalAwayScore { get; set; }

    public string BetStatus { get; set; }

    public int ChildMatchType { get; set; }

    public bool IsHalftimeCal { get; set; }

    public int SportType { get; set; }

    public int ConfirmFT { get; set; }

    public string EventStatus { get; set; }

    public int LeagueGroupId { get; set; }

    public int GameType { get; set; }

    public int ParentId { get; set; }

    public int AutoSetting { get; set; }
    public byte LeagueType { get; set; }

    public int ProviderCount { get; set; }
}