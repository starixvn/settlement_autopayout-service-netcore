using AutoPayout.Sportsbook.Criteria;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface LogSettleService
{
    Task WriteLog(SettleCriteria settleCriteria);
    Task WriteLogErrorSettle(Match match, string error);
    Task WriteLogResettle(Match match);
}

public class LogSettleServiceImp : LogSettleService
{
    private readonly IDynamicRepository repository;

    public LogSettleServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task WriteLog(SettleCriteria settleCriteria)
    {
        var criteria = new InsertLogCriteria
        {
            MatchId = settleCriteria.MatchId,
            Action = settleCriteria.Action,
            SportType = 1, //soccer
            IsAuto = true,
            SelectedResources = settleCriteria.SelectedResources
        };

        await repository.ExecuteAsync(criteria);

    }

    public async Task WriteLogResettle(Match match)
    {
        var resultDetailInfo = match.ResultDetails
            .Select(x => string.IsNullOrEmpty(x.AwayValue)
                ? $"{x.ResourceId}({x.HomeValue})"
                : $"{x.ResourceId}({x.HomeValue}-{x.AwayValue})");

        var criteria = new LogDirectlyCriteria
        {
            Doing = $"NAP: Direct Payout: {string.Join(",", resultDetailInfo.ToArray())}",
            Extend = match.MatchId,
        };

        await repository.ExecuteAsync(criteria);
    }

    public async Task WriteLogErrorSettle(Match match, string error)
    {
        var criteria = new LogErrorSettleCriteria
        {
            MatchId = match.MatchId,
            Remark = error,
            ActionId = 148 // calc-ear
        };

        await repository.ExecuteAsync(criteria);
    }
}
