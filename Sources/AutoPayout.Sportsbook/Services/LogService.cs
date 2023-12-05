namespace AutoPayout.Sportsbook.Services;

using Criteria;
using Models;

public interface LogService
{
    Task WriteLog(Match match, string actionName);

    Task WriteLogResettle(InsertLogCriteria criteria);
}

public class LogServiceImp : LogService
{
    private readonly IDynamicRepository repository;

    public LogServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task WriteLog(Match match, string actionName)
    {
        var criteria = new LogDirectlyCriteria
        {
            Extend = match.MatchId,
            Doing = GenerateLogData(match, actionName)
        };

        await repository.ExecuteAsync(criteria);
    }

    private static string GenerateLogData(Match match, string actionName)
    {
        var score = $"score: {match.FinalHomeScore ?? 0}-{match.FinalAwayScore ?? 0} HT: {match.HtHomeScore ?? 0}-{match.HtAwayScore ?? 0}";
        return $"NAP: Auto Payout Calculate({actionName}) for matchId: {match.MatchId} {score} GoalRecord: {match.GoalRecord} Remark({match.BetStatus})";
    }

    public async Task WriteLogResettle(InsertLogCriteria criteria)
    {
        await repository.ExecuteAsync(criteria);
    }
}