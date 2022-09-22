using AutoMapper;
using AutoPayout.Result.Models;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using Fanex.Settlement.ScaleOut.Constants;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly OtherInfoService otherInfoService;

        private readonly ScaleOutSettleService scaleOutSettleService;

        private readonly AutoPayoutResultService autoPayoutResultService;

        private readonly IMapper mapper;

        public MatchSettleServiceImp(
            MatchQueryService matchQueryService,
            MatchSettleCriteriaService matchSettleCriteriaService,
            IMapper mapper,
            OtherInfoService otherInfoService,
            ScaleOutSettleService scaleOutSettleService,
            AutoPayoutResultService autoPayoutResultService)
        {
            this.matchQueryService = matchQueryService;
            this.matchSettleCriteriaService = matchSettleCriteriaService;
            this.mapper = mapper;
            this.otherInfoService = otherInfoService;
            this.scaleOutSettleService = scaleOutSettleService;
            this.autoPayoutResultService = autoPayoutResultService;
        }

        public async Task<Models.Result> Settle(Match match, int sourceId)
        {
            var actionName = match.TypeId == SettleType.Halftime ? ActionName.CalculateHalfTime : ActionName.CalculateFullTime;

            var matchChecking = await matchQueryService.GetMatchInfo(match.MatchId);
            var resourceResult = GetResourceResult(match, matchChecking);
            var resultValidation = await ValidateBeforePushing(match, actionName, resourceResult);
            if (resultValidation.ErrCode == 0)
            {
                await scaleOutSettleService.UpdateUnderSettlementFlag(match.MatchId, 0, actionName);
                await autoPayoutResultService.Insert(match, sourceId, MatchStatus.Waiting);

                await Execute(match, matchChecking, resourceResult, sourceId, actionName);

                await UpdateAutoRefundChildMatch(match, actionName);
                await SettleHedgeMatch(match, matchChecking, resourceResult, actionName, sourceId);
            }

            return resultValidation;
        }

        private async Task SettleHedgeMatch(
            Match match,
            MatchChecking matchChecking,
            string resourceResult,
            string actionName,
            int sourceId)
        {
            var hedgeMatch = await matchQueryService.GetHedgeMatchId(match.MatchId);
            if (hedgeMatch.MatchId > 0 && !hedgeMatch.IsUndecided)
            {
                match.MatchId = hedgeMatch.MatchId;
                await scaleOutSettleService.UpdateUnderSettlementFlag(match.MatchId, 0, actionName);
                await autoPayoutResultService.Insert(match, sourceId, MatchStatus.Waiting);
                await Execute(match, matchChecking, resourceResult, sourceId, actionName);
            }
        }

        private async Task UpdateMatchInfo(Match match, MatchSettleCriteria criteria)
        {
            var updateMatchInfoRequest = new UpdateMatchInfoRequest
            {
                MatchId = criteria.MatchId,
                Action = criteria.Action,
                HalfTimeHome = match.HtHomeScore,
                HalfTimeAway = match.HtAwayScore,
                FullTimeHome = match.FinalHomeScore,
                FullTimeAway = match.FinalAwayScore,
                GoalRecord = criteria.GoalRecord,
                BetStatus = criteria.BetStatus,
                IsAuto = true,
                ResourceResult = criteria.ResourceResult
            };

            await scaleOutSettleService.UpdateMatchInfo(updateMatchInfoRequest);
        }

        private async Task UpdateAutoRefundChildMatch(Match match, string actionName)
        {
            var request = new RefundChildMatchRequest
            {
                ParentId = match.MatchId,
                ActionName = actionName,
                LeagueGroupId = match.LeagueGroupId
            };

            if (match.TypeId == SettleType.Halftime)
            {
                request.HomeValue = match.HtHomeScore;
                request.AwayValue = match.HtAwayScore;
            }
            else
            {
                request.HomeValue = match.FinalHomeScore;
                request.AwayValue = match.FinalAwayScore;
            }

            await scaleOutSettleService.UpdateAutoRefundChildMatch(request);
        }

        private async Task<Models.Result> ValidateBeforePushing(Match match, string actionName, string resourceResult)
        {
            var request = new EventTicketRequest
            {
                MatchId = match.MatchId,
                Mode = match.TypeId == SettleType.Halftime ? Mode.HalfTime : Mode.FullTime,
                Action = actionName,
                IsAuto = true,
                SpName = "NAP_Admin_Settlement_1_AutoPayout_Push",
                ResourceResult = resourceResult
            };

            var result = await scaleOutSettleService.ValidateEventAndTicket(request);

            return new Models.Result
            {
                ErrCode = result.ErrorCode,
                ErrMsg = result.ErrorMessage
            };
        }

        private string GetResourceResult(Match match, MatchChecking matchChecking)
        {
            var goal = mapper.Map<MatchChecking, Goal>(matchChecking);
            goal.GoalRecord = match.GoalRecord;

            return matchQueryService.GetResource(match.TypeId, goal);
        }

        private async Task Execute(Match match, MatchChecking matchChecking, string resourceResult, int sourceId, string actionName)
        {
            var criteria = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);
            criteria.SourceId = sourceId;
            criteria.ResourceResult = resourceResult;
            criteria.RefResult = await GetRelatedInfo(match);
            await UpdateMatchInfo(match, criteria);
            matchQueryService.ExecuteAllSiteDb(criteria);
        }

        private async Task<string> GetRelatedInfo(Match match)
        {
            if (match.IsTotalCornerGoalMatch)
            {
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