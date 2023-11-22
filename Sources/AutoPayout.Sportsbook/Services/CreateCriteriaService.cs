using AutoMapper;
using AutoPayout.Result.Constants;
using AutoPayout.Result.Models;
using AutoPayout.Result.Services;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services;

public interface CreateCriteriaService
{
    MatchSettleCriteria CreateSettleCriteria(Match match, MatchChecking matchChecking);

    InsertLogCriteria CreateLogCriteria(Match match, MatchChecking matchChecking, string actionName);
}

public class CreateCriteriaServiceImp : CreateCriteriaService
{
    private readonly BetStatusHalftimeService betStatusHalftimeService;

    private readonly BetStatusFullTimeService betStatusFullTimeService;

    private readonly ActionResourceService actionResourceService;

    private readonly XmlConvertService xmlConvertService;

    private readonly IMapper mapper;

    public CreateCriteriaServiceImp(
        BetStatusHalftimeService betStatusHalftimeService,
        BetStatusFullTimeService betStatusFullTimeService,
        ActionResourceService actionResourceService,
        XmlConvertService xmlConvertService,
        IMapper mapper)
    {
        this.betStatusHalftimeService = betStatusHalftimeService;
        this.betStatusFullTimeService = betStatusFullTimeService;
        this.actionResourceService = actionResourceService;
        this.xmlConvertService = xmlConvertService;
        this.mapper = mapper;
    }

    public MatchSettleCriteria CreateSettleCriteria(Match match, MatchChecking matchChecking)
    {
        if (match.TypeId == SettleType.Halftime)
        {
            var betStatus = BetStatusHalftime(match, matchChecking);
            return new MatchSettleHalfTimeCriteria(match, betStatus);
        }
        else
        {
            var betStatus = BetStatusFullTime(match, matchChecking);
            return match.IsSabaPenaltyShootout
                ? new MatchSettleAllCriteria(match, betStatus)
                : new MatchSettleFullTimeCriteria(match, betStatus);
        }
    }

    private string BetStatusHalftime(Match match, MatchChecking matchChecking)
    {
        var betStatus = matchChecking.IsExistHtFgLg ? betStatusHalftimeService.BetStatus(match.BetStatus, match.GoalRecord) : match.BetStatus;

        if (matchChecking.IsExistFgNg)
        {
            var fullTimeBetStatus = betStatusHalftimeService.FullTimeBetStatus(match.GoalRecord);

            return betStatusHalftimeService.AppendFullTimeBetStatus(betStatus, fullTimeBetStatus);
        }

        return betStatus;
    }

    private string BetStatusFullTime(Match match, MatchChecking matchChecking)
    {
        return matchChecking.IsExistFgLg ? betStatusFullTimeService.BetStatus(match.BetStatus, match.GoalRecord) : match.BetStatus;
    }

    public InsertLogCriteria CreateLogCriteria(Match match, MatchChecking matchChecking, string actionName)
    {
        const int soccer = 1;
        var goal = mapper.Map<MatchChecking, Goal>(matchChecking);
        goal.GoalRecord = match.GoalRecord;

        var selectedResources = GetLogResource(match, goal);
        return new InsertLogCriteria
        {
            MatchId = match.MatchId,
            Action = actionName,
            SportType = soccer,
            SelectedResources = selectedResources
        };
    }

    private string GetLogResource(Match match, Goal goal)
    {
        List<XmlResource> resources;
        ResourceItem logResource;
        if (match.TypeId == SettleType.Halftime)
        {
            resources = actionResourceService.HalftimeResources(goal);
            logResource = actionResourceService.CalculateLogResources(ResourceKey.Halftime, match.HtHomeScore.ToString(), match.HtAwayScore.ToString());
        }
        else
        {
            resources = actionResourceService.FullTimeGoalResources(goal);
            logResource = actionResourceService.CalculateLogResources(ResourceKey.Final, match.FinalHomeScore.ToString(), match.FinalAwayScore.ToString());
        }

        var resourcesMapper = mapper.Map<List<ResourceItem>>(resources);
        resourcesMapper.Add(logResource);

        return xmlConvertService.Serialize(new Resources
        {
            ResourceItems = resourcesMapper
        });
    }
}