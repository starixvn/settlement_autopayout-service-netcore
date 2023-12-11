namespace AutoPayout.Sportsbook.Services;

using Constants;
using Criteria;
using Models;

public interface InsertInfoService
{
    Task InsertResult(Match match, int source, int status);

    Task InsertMessageToLetstalk(int matchId);
}

public class InsertInfoServiceImp : InsertInfoService
{
    private readonly IDynamicRepository repository;

    public InsertInfoServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task InsertResult(Match match, int source, int status)
    {
        var criteria = new AddAutoPayoutResultCriteria
        {
            MatchId = match.MatchId,
            TypeId = match.TypeId,
            SourceId = source,
            StatusId = status
        };

        if (match.TypeId == SettleType.Halftime)
        {
            criteria.HomeValue = match.HtHomeScore ?? 0;
            criteria.AwayValue = match.HtAwayScore ?? 0;
        }
        else
        {
            criteria.HomeValue = match.FinalHomeScore ?? 0;
            criteria.AwayValue = match.FinalAwayScore ?? 0;
        }

        await repository.ExecuteAsync(criteria);
    }

    public async Task InsertMessageToLetstalk(int matchId)
    {
        const string message = "The match has been completed at @latest_settle_time with Auto-Settle FM turned OFF";
        var criteria = new AddNotificationCriteria
        {
            MatchId = matchId,
            ActionName = ActionName.CalculateFullTime,
            MessageContent = message
        };
        await repository.ExecuteAsync(criteria);
    }
}