using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using Fanex.Settlement.Converter.Services;

namespace DirectPayout.Early.Services;

public interface MatchSettleController
{
    Task Process(Match match);
}

public class MatchSettleControllerImp(
    MatchSettlingPoolService matchSettlingPoolService,
    ValidationService validationService,
    XmlConvertService xmlConvertService,
    SettleService settleService,
    AutoPayoutResultService autoPayoutResultService,
    LogSettleService logSettleService)
    : MatchSettleController
{
    public async Task Process(Match match)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(20)); // Simulate processing time
            await validationService.VerifyNoRunningOdds(match);

            var settleCriteria = CreateSettleCriteria(match);
            await validationService.VerifyEventAndTicket(settleCriteria);

            await settleService.Settle(match, settleCriteria);
        }
        catch (AppException exception)
        {
            await HandleAutoPayoutException(match, exception);
        }
        finally
        {
            matchSettlingPoolService.Remove(match);
        }
    }

    private SettleCriteria CreateSettleCriteria(Match match)
    {
        var resources = new Resources<ResultDetail>
        {
            ResourceItems = match.ResultDetails
        };

        return new SettleCriteria
        {
            MatchId = match.MatchId,
            SelectedResources = xmlConvertService.Serialize(resources),
            Action = "CAL",
            IsAuto = true
        };
    }

    private async Task HandleAutoPayoutException(Match match, AppException exception)
    {
        var settleCriteria = CreateSettleCriteria(match);
        var resultDetailInfo = match.ResultDetails.Select(x => x.ResourceId);
        var errorMessage = $"Failed Direct Payout: Result ({string.Join(",", resultDetailInfo.ToArray())}) {exception.Message}";

        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Failed, errorMessage);
        await logSettleService.WriteLogErrorSettle(match, errorMessage);
    }
}