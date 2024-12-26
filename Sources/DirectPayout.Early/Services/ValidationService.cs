using AutoPayout.Core.Models;
using DirectPayout.Early.Constants;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.Extensions.Options;
using Match = DirectPayout.Early.Models.Match;
using ResultDetail = DirectPayout.Early.Models.ResultDetail;

namespace DirectPayout.Early.Services;

public interface ValidationService
{
    Task VerifyNoRunningOdds(Match match);

    Task VerifyEventAndTicket(SettleCriteria criteria);

    Task VerifyRelatedResults(Match match);
}

public class ValidationServiceImp(
    IDynamicRepository repository,
    XmlConvertService xmlConvertService,
    ScaleOutSettleService scaleOutSettleService,
    IOptions<AppSettingsKey> appSettings,
    AutoPayoutResultService autoPayoutResultService,
    LogSettleService logSettleService)
    : ValidationService
{
    public async Task VerifyEventAndTicket(SettleCriteria criteria)
    {
        var result = new Result();
        var request = new EventTicketRequest
        {
            MatchId = criteria.MatchId,
            Action = criteria.Action,
            SpName = criteria.SpName,
            Mode = "EAR",
            IsAuto = true,
            SelectedResources = criteria.SelectedResources
        };

        for (var i = 0; i < appSettings.Value.RetryCount; i++)
        {
            var resultMessage = await scaleOutSettleService.ValidateEventAndTicket(request);
            if (resultMessage.ErrorCode == ErrorCodes.MatchInQueue && i < appSettings.Value.RetryCount - 1)
            {
                await Task.Delay(appSettings.Value.RetryInterval);
                continue;
            }

            result = new Result
            {
                ErrCode = resultMessage.ErrorCode,
                ErrMsg = resultMessage.ErrorMessage
            };
        }

        if (result.ErrCode != 0)
        {
            throw new AppException(result.ErrMsg);
        }
    }

    public async Task VerifyNoRunningOdds(Match match)
    {
        var selectedResource = CreateMatchResources(match);

        var criteria = new VerifyNoRunningOddsCriteria
        {
            MatchId = match.MatchId,
            SelectedResource = selectedResource,
            Action = "CAL_EAR",
            CheckType = 0
        };

        await repository.ExecuteAsync(criteria);

        if (criteria.ErrCode > 0)
        {
            throw new RunningOddsException();
        }
    }

    public async Task VerifyRelatedResults(Match match)
    {
        foreach (var item in match.ResultDetails)
        {
            if (string.IsNullOrEmpty(item.RelatedCalResources) && string.IsNullOrEmpty(item.RelatedSettResources))
            {
                continue;
            }

            var isValid = await IsValidRelatedResult(match, item);

            item.IsInvalid = !isValid;
            item.Expired = !isValid && item.Expired;
        }

        await HandleExpiredResults(match);

        match.ResultDetails = match.ResultDetails.Where(x => !x.IsInvalid).ToList();

        if (match.ResultDetails.Count == 0)
        {
            throw new IgnoreSettlementException();
        }
    }

    private async Task<bool> IsValidRelatedResult(Match match, ResultDetail item)
    {
        var criteria = new CheckRelatedResultsCriteria
        {
            MatchId = match.MatchId,
            SportType = 1,
            RelatedCalResources = item.RelatedCalResources ?? string.Empty,
            RelatedSettResources = item.RelatedSettResources ?? string.Empty
        };

        var relatedResults = await repository.FetchAsync<ResultDetail>(criteria);

        var isExistsRelatedResults = TryGetAllExistsResults(match.ResultDetails, relatedResults.ToList(), out var existingResourceIds);

        var checkingRelatedResults = relatedResults
            .Where(x => !existingResourceIds.Contains(x.ResourceId))
            .ToList();

        var isCalRelatedResults = IsCalRelatedResults(checkingRelatedResults, item.RelatedCalResourceIds);
        var isSettleRelatedResults = IsSettleRelatedResults(checkingRelatedResults, item.RelatedSettResourcesIds);

        return isExistsRelatedResults || (isCalRelatedResults && isSettleRelatedResults);
    }

    private static bool IsCalRelatedResults(List<ResultDetail> relatedResults, List<string> relatedCalResourceIds)
    {
        var flag = relatedResults.TrueForAll(x => relatedCalResourceIds.Contains(x.ResourceId) && x.SettlementStatusId == SettlementStatusId.Calculated);
        return relatedResults.Count == 0
               || relatedCalResourceIds.Count == 0
               || relatedResults.Count > 0 && flag;
    }

    private static bool IsSettleRelatedResults(List<ResultDetail> relatedResults, List<string> relatedSettResourcesIds)
    {
        var flag = relatedResults.TrueForAll(x => relatedSettResourcesIds.Contains(x.ResourceId) && (x.ResultStatusId is ResultStatusId.ManualCompleted or ResultStatusId.AutoCompleted));
        return relatedResults.Count == 0
               || relatedSettResourcesIds.Count == 0
               || relatedResults.Count > 0 && flag;
    }

    private static bool TryGetAllExistsResults(List<ResultDetail> settlingResults, List<ResultDetail> relatedResults, out List<string> existingResourceIds)
    {
        var settlingResultsIds = settlingResults.Select(x => x.ResourceId).ToList();

        existingResourceIds = relatedResults
            .Where(x => settlingResultsIds.Contains(x.ResourceId))
            .Select(x => x.ResourceId).ToList();

        return existingResourceIds.Count == relatedResults.Count;
    }

    private async Task HandleExpiredResults(Match match)
    {
        var expiredResults = match.ResultDetails.Where(x => x.IsInvalid && x.Expired).ToList();
        if (expiredResults.Count != 0)
        {
            var errorMessage = string.Format(ErrorMsg.ExpiredResults, string.Join(",", expiredResults.Select(x => x.ToString())));
            await autoPayoutResultService.UpdAutoPayoutFailResult(new Match { MatchId = match.MatchId, ResultDetails = expiredResults }, errorMessage);
            await logSettleService.WriteLogErrorSettle(match, errorMessage);
        }
    }

    private string CreateMatchResources(Match match)
    {
        var resourceIds = match.ResultDetails.Select(resultDetail => resultDetail.ResourceId).ToList();

        var checkOddsItem = new CheckOddsItem
        {
            MatchId = match.MatchId,
            ResourceIds = resourceIds
        };

        var resources = new Resources<CheckOddsItem>
        {
            ResourceItems = [checkOddsItem]
        };

        return xmlConvertService.Serialize(resources);
    }
}