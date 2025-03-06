using AutoPayout.Result.Constants;
using AutoPayout.Result.Models;
using AutoPayout.Result.Services;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using Fanex.Settlement.MessageBridge.JackTime;
using Fanex.Settlement.ScaleOut.Request;

namespace AutoPayout.Sportsbook.Services;

public interface CreateCriteriaService
{
    MatchSettleCriteria CreateSettleCriteria(Match match, MatchChecking matchChecking);

    InsertLogCriteria CreateLogCriteria(Match match, MatchChecking matchChecking, string actionName);

    EventTicketRequest CreateEventTicketRequest(Match match, string actionName, string resourceResult);

    SoccerResult CreateRequestJT(Match match);
}

public class CreateCriteriaServiceImp(
    BetStatusHalftimeService betStatusHalftimeService,
    BetStatusFullTimeService betStatusFullTimeService,
    ActionResourceService actionResourceService,
    XmlConvertService xmlConvertService,
    IMapper mapper)
    : CreateCriteriaService
{
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
            return (match.IsSabaPenaltyShootout || match.IsNthCorner)
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
        var goal = mapper.Map<MatchChecking, Goal>(matchChecking);
        goal.GoalRecord = match.GoalRecord;

        var selectedResources = GetLogResource(match, goal);
        return new InsertLogCriteria
        {
            MatchId = match.MatchId,
            Action = actionName,
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
            logResource = actionResourceService.CalculateResources(ResourceKey.Halftime, match.HtHomeScore.ToString(), match.HtAwayScore.ToString());
        }
        else
        {
            resources = actionResourceService.FullTimeGoalResources(goal);
            logResource = actionResourceService.CalculateResources(ResourceKey.Final, match.FinalHomeScore.ToString(), match.FinalAwayScore.ToString());
        }

        var resourcesMapper = mapper.Map<List<ResourceItem>>(resources);
        resourcesMapper.Add(logResource);

        return xmlConvertService.Serialize(new Resources
        {
            ResourceItems = resourcesMapper
        });
    }

    public EventTicketRequest CreateEventTicketRequest(Match match, string actionName, string resourceResult)
    {

        var selectedResource = xmlConvertService.Serialize(new Resources
        {
            ResourceItems = actionResourceService.CreateResourceByAction(match.TypeId)
        });

        return new EventTicketRequest
        {
            MatchId = match.MatchId,
            Mode = match.TypeId == SettleType.Halftime ? Mode.HalfTime : Mode.FullTime,
            Action = actionName,
            IsAuto = true,
            SpName = "NAP_Admin_Settlement_1_AutoPayout_Push",
            ResourceResult = resourceResult,
            SelectedResources = selectedResource
        };
    }

    public SoccerResult CreateRequestJT(Match match)
    {
        return new SoccerResult
        {
            MatchId = match.MatchId,
            Action = "CAL",
            SelectedResources = CreateResourcesJT(match)
        };
    }


    private string CreateResourcesJT(Match match)
    {
        var resourceCalculated = new List<ResourceItem>();
        var matchPenaltyOrCorner = match.IsSabaPenaltyShootout || match.IsNthCorner;

        if (match.TypeId == SettleType.Halftime || !matchPenaltyOrCorner)
        {
            resourceCalculated.Add(actionResourceService.CalculateResources(ResourceKey.Halftime,
                match.HtHomeScore.ToString(), match.HtAwayScore.ToString()));
        }

        if (match.TypeId != SettleType.Halftime)
        {
            resourceCalculated.Add(actionResourceService.CalculateResources(ResourceKey.Final, match.FinalHomeScore.ToString(), match.FinalAwayScore.ToString()));
        }


        return xmlConvertService.Serialize(new Resources
        {
            ResourceItems = resourceCalculated
        });
    }
}