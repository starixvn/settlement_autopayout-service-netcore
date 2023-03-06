using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services;
public interface LogDirectlyService
{
    Task WriteLog(Match match, string actionName);
}

public class LogDirectlyServiceImp : LogDirectlyService
{
    private readonly IDynamicRepository repository;

    public LogDirectlyServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task WriteLog(Match match, string actionName)
    {
        var criteria = new LogDirectlyCriteria
        {
            Extend = match.MatchId,
            Doing = ""
        };

        await repository.ExecuteAsync(criteria);
    }

    private string GenerateLogData(Match match, string actionName)
    {
        //var scoreInfo = match.TypeId == Se
        var message = $"NAP:Calculate({actionName}) for matchid: {match.MatchId} score: ${match.HtHomeScore}-{match.HtAwayScore} HT: ${match.FinalHomeScore}-{match.FinalAwayScore} GoalRecord: {match.GoalRecord}(Old: ) Remark({match.BetStatus})";

        return message;
    }
}