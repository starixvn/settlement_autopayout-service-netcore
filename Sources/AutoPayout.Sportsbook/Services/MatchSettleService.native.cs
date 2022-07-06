using AutoMapper;
using AutoPayout.Result.Models;
using AutoPayout.Result.Services;
using Kaden.XmlConverter;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace AutoPayout.Sportsbook.Services
{
    public interface MatchSettleService
    {
        Task<Models.Result> Settle(Match match, int sourceId);
    }

    public class MatchSettleServiceImp : MatchSettleService
    {
        private readonly MatchQueryService matchQueryService;

        private readonly MatchSettleCriteriaService matchSettleCriteriaService;

        private readonly ActionResourceService actionResourceService;

        private readonly XmlConvertService xmlConvertService;

        private readonly IMapper mapper;

        private readonly OtherInfoService otherInfoService;

        public MatchSettleServiceImp(
            MatchQueryService matchQueryService,
            MatchSettleCriteriaService matchSettleCriteriaService,
            ActionResourceService actionResourceService,
            XmlConvertService xmlConvertService,
            IMapper mapper,
            OtherInfoService otherInfoService)
        {
            this.matchQueryService = matchQueryService;
            this.matchSettleCriteriaService = matchSettleCriteriaService;
            this.actionResourceService = actionResourceService;
            this.xmlConvertService = xmlConvertService;
            this.mapper = mapper;
            this.otherInfoService = otherInfoService;
        }

        public async Task<Models.Result> Settle(Match match, int sourceId)
        {
            var result = await Execute(match, sourceId);

            if (result.ErrCode == 0)
            {
                return await HedgeMatch(match, sourceId);
            }

            return result;
        }

        private async Task<Models.Result> HedgeMatch(Match match, int sourceId)
        {
            var hedgeMatch = await matchQueryService.GetHedgeMatchId(match.MatchId);
            if (hedgeMatch.MatchId > 0 && !hedgeMatch.IsUndecided)
            {
                match.MatchId = hedgeMatch.MatchId;
                return await Execute(match, sourceId);
            }

            return new Models.Result();
        }

        private async Task<Models.Result> Execute(Match match, int sourceId)
        {
            var matchChecking = await matchQueryService.GetMatchInfo(match.MatchId);
            var resourceResult = GetResource(match, matchChecking);
            var criteria = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);
            criteria.SourceId = sourceId;
            criteria.ResourceResult = resourceResult;
            criteria.RefResult = await GetRelatedInfo(match);
            return await matchQueryService.Execute(criteria);
        }

        private string GetResource(Match match, MatchChecking matchChecking)
        {
            var goal = mapper.Map<MatchChecking, Goal>(matchChecking);
            goal.GoalRecord = match.GoalRecord;

            var resources = match.TypeId == SettleType.Halftime
                ? actionResourceService.HalftimeResources(goal)
                : actionResourceService.FullTimeGoalResources(goal);

            return SerializeObjectToString(resources);
        }

        private string SerializeObjectToString(List<XmlResource> resources)
        {
            if (resources.Count > 0)
            {
                var xmlResults = new XmlResults { Resources = resources };
                return xmlConvertService.Serialize(xmlResults);
            }
            else
            {
                return string.Empty;
            }
        }

        private async Task<string> GetRelatedInfo(Match match)
        {
            if (match.IsTotalCornerGoalMatch) {
                var resourceId = match.TypeId == SettleType.Halftime ? ResourceId.HalfTime : ResourceId.FullTime;
                var (_, resultDetail) = await matchQueryService.GetMatchAndResultDetail(match.MatchId);
                var resultsChecking = resultDetail.Where(result => result.ResourceId == resourceId).ToList();
                var (mainScore, cornerScore) = otherInfoService.GetRelatedMatchesScore(resultsChecking);

                return $"{mainScore}; {cornerScore}";
            }

            return null;
        }
    }
}