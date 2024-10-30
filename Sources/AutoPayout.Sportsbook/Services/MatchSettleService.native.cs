using AutoPayout.Result.Models;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;

namespace AutoPayout.Sportsbook.Services;

using Constants;
using Criteria;
using Models;

public interface MatchSettleService
{
	Task<Result> Settle(Match match, int sourceId);
}

public class MatchSettleServiceImp(
	MatchQueryService matchQueryService,
	CreateCriteriaService createCriteriaService,
	IMapper mapper,
	OtherInfoService otherInfoService,
	ScaleOutSettleService scaleOutSettleService,
	InsertInfoService insertInfoService,
	LogService logService)
	: MatchSettleService
{
	public async Task<Result> Settle(Match match, int sourceId)
	{
		var actionName = match.TypeId == SettleType.Halftime ? ActionName.CalculateHalfTime : ActionName.CalculateFullTime;

		var matchChecking = await matchQueryService.GetMatchInfo(match.MatchId);
		var resourceResult = GetResourceResult(match, matchChecking);
		var resultValidation = await ValidateBeforePushing(match, actionName, resourceResult);
		if (resultValidation.ErrCode == 0)
		{
			var insertLogCriteria = createCriteriaService.CreateLogCriteria(match, matchChecking, actionName);
			await WriteLog(match, insertLogCriteria, actionName);
			await scaleOutSettleService.UpdateUnderSettlementFlag(match.MatchId, 0, actionName);

			await insertInfoService.InsertResult(match, sourceId, MatchStatus.Waiting);

			await Execute(match, matchChecking, resourceResult, sourceId);

			await UpdateAutoRefundChildMatch(match, actionName);

			await InsertMessageToLetstalk(match);

			await SettleHedgeMatch(match, matchChecking, resourceResult, actionName, sourceId);
		}

		return resultValidation;
	}

	private async Task InsertMessageToLetstalk(Match match)
	{
		if (match.IsAutoFastMarketSettlement || match.TypeId == SettleType.Halftime)
		{
			return;
		}

		var isExistedTicketRunning = await matchQueryService.IsExistedFastMarketTicketRunning(match.MatchId);
		if (isExistedTicketRunning)
		{
			await insertInfoService.InsertMessageToLetstalk(match.MatchId);
		}
	}

	private async Task WriteLog(Match match, InsertLogCriteria insertLogCriteria, string actionName)
	{
		await logService.WriteLogResettle(insertLogCriteria);

		await logService.WriteLog(match, actionName);
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
			await insertInfoService.InsertResult(match, sourceId, MatchStatus.Waiting);
			await Execute(match, matchChecking, resourceResult, sourceId);
		}
	}

	private async Task UpdateMatchInfo(MatchSettleCriteria criteria)
	{
		var updateMatchInfoRequest = mapper.Map<MatchSettleCriteria, UpdateMatchInfoRequest>(criteria);
		updateMatchInfoRequest.IsAuto = true;

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

	private async Task<Result> ValidateBeforePushing(Match match, string actionName, string resourceResult)
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

		return new Result
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

	private async Task Execute(Match match, MatchChecking matchChecking, string resourceResult, int sourceId)
	{
		var criteria = createCriteriaService.CreateSettleCriteria(match, matchChecking);
		criteria.SourceId = sourceId;
		criteria.ResourceResult = resourceResult;
		criteria.RefResult = await GetRelatedInfo(match);

		await UpdateMatchInfo(criteria);
		matchQueryService.ExecuteAllSiteDb(criteria);
		await insertInfoService.UpdateAutoResult(match, MatchStatus.Completed, sourceId);

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

		return string.Empty;
	}
}