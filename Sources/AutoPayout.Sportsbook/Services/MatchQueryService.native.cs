namespace AutoPayout.Sportsbook.Services;

using AutoPayout.Result.Models;
using AutoPayout.Result.Services;
using Constants;
using Criteria;
using Models;

public interface MatchQueryService
{
    Task<List<Match>> GetWaitingMatches();

    Task<MatchHedgeDetail> GetHedgeMatchId(int matchId);

    Task<MatchChecking> GetMatchInfo(int matchId);

    string GetResource(int typeId, Goal goal);

    void ExecuteAllSiteDb(MatchSettleCriteria criteria);

    Task<(RemarkSettingModel matchRemark, List<ResultDetail> resultDetail)> GetMatchAndResultDetail(int matchId);

    Task<bool> IsExistedFastMarketTicketRunning(int matchId);
}

public class MatchQueryServiceImp(
    IDynamicRepository repository,
    ActionResourceService actionResourceService,
    XmlConvertService xmlConvertService,
    IOptions<AppSettingsKey> appSettingsKey)
    : MatchQueryService
{
    public async Task<List<Match>> GetWaitingMatches()
    {
        var criteria = new MatchListingCriteria();
        return (await repository.FetchAsync<Match>(criteria)).ToList();
    }

    public async Task<MatchHedgeDetail> GetHedgeMatchId(int matchId)
    {
        var criteria = new HedgeMatchQueryCriteria { MatchId = matchId };
        var match = await repository.GetAsync<MatchHedgeDetail>(criteria);
        return match ?? new MatchHedgeDetail();
    }

    public async Task<MatchChecking> GetMatchInfo(int matchId)
    {
        var criteria = new MatchCheckingCriteria { MatchId = matchId };
        var listMatchChecking = new List<MatchChecking>();
        foreach (var server in appSettingsKey.Value.SiteDbServer)
        {
            criteria.DbServer = server;
            var item = await repository.GetAsync<MatchChecking>(criteria) ?? new MatchChecking();
            listMatchChecking.Add(item);
        }

        return new MatchChecking
        {
            IsExistHtFgLg = listMatchChecking.Exists(item => item.IsExistHtFgLg),
            IsExistFgLg = listMatchChecking.Exists(item => item.IsExistFgLg),
            IsExistFgNg = listMatchChecking.Exists(item => item.IsExistFgNg),
            IsEnabledGoalRecordFt = listMatchChecking.Exists(item => item.IsEnabledGoalRecordFt),
            IsEnabledGoalRecordHt = listMatchChecking.Exists(item => item.IsEnabledGoalRecordHt)
        };
    }

    public void ExecuteAllSiteDb(MatchSettleCriteria criteria)
    {
        foreach (var server in appSettingsKey.Value.SiteDbServer)
        {
            criteria.DbServer = server;
            repository.Execute(criteria);
        }
    }

    public async Task<(RemarkSettingModel matchRemark, List<ResultDetail> resultDetail)> GetMatchAndResultDetail(int matchId)
    {
        var criteria = new MatchDetailCriteria { MatchId = matchId };
        var (matchRemark, resultDetail) = await repository.FetchMultipleAsync<MatchRemark, ResultDetail>(criteria);

        return (matchRemark.FirstOrDefault()?.Remark, resultDetail.ToList());
    }

    public string GetResource(int typeId, Goal goal)
    {
        var resources = typeId == SettleType.Halftime
            ? actionResourceService.HalftimeResources(goal)
            : actionResourceService.FullTimeGoalResources(goal);

        return SerializeObjectToString(resources);
    }

    private string SerializeObjectToString(List<XmlResource> resources)
    {
        if (resources.Count <= 0)
        {
            return string.Empty;
        }

        var xmlResults = new XmlResults { Resources = resources };
        return xmlConvertService.Serialize(xmlResults);
    }

    public async Task<bool> IsExistedFastMarketTicketRunning(int matchId)
    {
        var runningTicket = 0;
        var criteria = new GetFastMarketTicketDetailCriteria { MatchId = matchId };
        foreach (var server in appSettingsKey.Value.SiteDbServer)
        {
            criteria.DbServer = server;
            var ticketCounts = await repository.GetAsync<TicketCounts>(criteria);
            runningTicket += ticketCounts?.BetRunningCount ?? 0;
        }

        return runningTicket > 0;
    }
}