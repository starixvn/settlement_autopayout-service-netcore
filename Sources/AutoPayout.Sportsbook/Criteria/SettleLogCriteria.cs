using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Criteria;

public class SettleLogCriteria : NonQueryCommand
{
    public SettleLogCriteria(Match match, AutoPayoutException exception)
    {
        MatchId = match.MatchId;
        GoalRecord = match.GoalRecord;
        Remark = exception.Description ?? string.Empty;
        SetScore(match);
    }

    public SettleLogCriteria(Match match, string errorMessage)
    {
        MatchId = match.MatchId;
        GoalRecord = match.GoalRecord;
        ActionId = match.TypeId == SettleType.FullTime ? Core.Constants.ActionId.FullTypeAfterSettleFail : Core.Constants.ActionId.HalfTypeAfterSettleFail;
        Remark = errorMessage;
    }

    public int UserId { get; set; }

    public int MatchId { get; set; }

    public int ActionId { get; set; }

    public int? FinalHomeScore { get; set; }

    public int? FinalAwayScore { get; set; }

    public int? HtHomeScore { get; set; }

    public int? HtAwayScore { get; set; }

    public string Remark { get; set; }

    public string GoalRecord { get; set; }

    public string Ip { get; set; }

    public override bool IsValid() => MatchId > 0;

    public override string GetSettingKey() => "WriteLog_UserLog_Settle";

    private void SetScore(Match match)
    {
        if (match.TypeId == SettleType.Halftime)
        {
            ActionId = Core.Constants.ActionId.HalfType;
            HtHomeScore = match.HtHomeScore ?? 0;
            HtAwayScore = match.HtAwayScore ?? 0;
        }
        else
        {
            ActionId = Core.Constants.ActionId.FullType;
            HtHomeScore = match.HtHomeScore ?? 0;
            HtAwayScore = match.HtAwayScore ?? 0;
            FinalHomeScore = match.FinalHomeScore ?? 0;
            FinalAwayScore = match.FinalAwayScore ?? 0;
        }
    }
}