using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services;

public interface LogService
{
    Task WriteLog(Match match, string actionName);

    Task WriteLog(InsertLogCriteria criteria);
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
        try
        {
            var criteria = new LogDirectlyCriteria
            {
                Extend = match.MatchId,
                Doing = GenerateLogData(match, actionName)
            };

            await repository.ExecuteAsync(criteria);
        }
        catch (Exception e)
        {
            await Fanex.Logging.LogService.ErrorAsync("Fanex.AutoPayout.Service(Write Log Directly): ", e);
        }
    }

    private static string GenerateLogData(Match match, string actionName)
    {
        var score = $"score: {match.FinalHomeScore ?? 0}-{match.FinalAwayScore ?? 0} HT: {match.HtHomeScore ?? 0}-{match.HtAwayScore ?? 0}";
        var message = $"NAP: Auto Payout Calculate({actionName}) for matchId: {match.MatchId} {score} GoalRecord: {match.GoalRecord} Remark({match.BetStatus})";

        return message;
    }

    public async Task WriteLog(InsertLogCriteria criteria)
    {
        try
        {
            await repository.ExecuteAsync(criteria);
        }
        catch (Exception e)
        {
            await Fanex.Logging.LogService.ErrorAsync("Fanex.AutoPayout.Service(Write resettle Log): ", e);
        }
    }
}