using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using Fanex.Logging;

namespace AutoPayout.Sportsbook.Services;
public interface LogDirectlyService
{
    void WriteLog(Match match, string actionName);
}

public class LogDirectlyServiceImp : LogDirectlyService
{
    private readonly IDynamicRepository repository;

    public LogDirectlyServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public void WriteLog(Match match, string actionName)
    {
        try
        {
            var criteria = new LogDirectlyCriteria
            {
                Extend = match.MatchId,
                Doing = GenerateLogData(match, actionName)
            };

            repository.Execute(criteria);
        }
        catch (Exception e)
        {
            LogService.Error("Fanex.AutoPayout.Service(Write Log Directly): ", e);
        }

    }

    private string GenerateLogData(Match match, string actionName)
    {
        var message = $"NAP:Calculate({actionName}) for matchid: {match.MatchId} score: ${match.HtHomeScore}-{match.HtAwayScore} HT: ${match.FinalHomeScore}-{match.FinalAwayScore} GoalRecord: {match.GoalRecord}(Old: ) Remark({match.BetStatus})";

        return message;
    }
}