using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Criteria;

public class MatchSettleCriteria : NonQueryCommand
{
    public MatchSettleCriteria(Match match)
    {
        AdminId = 0;
        ErrCode = 0;
        ErrMsg = string.Empty;
        MatchId = match.MatchId;
        GoalRecord = match.GoalRecord;
    }

    public int AdminId { get; set; }

    public int MatchId { get; set; }

    public string Action { get; set; }

    public string BetStatus { get; set; }

    public string GoalRecord { get; set; }

    public string ResourceResult { get; set; }

    public int SourceId { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public string RefResult { get; set; }

    public string BroadcastSessionId { get; } = Guid.NewGuid().ToString();

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override bool IsValid() => MatchId > 0;

    public override string GetSettingKey() => $"NAP_Admin_Settlement_1_AutoPayout_Push_{DbServer}";
}