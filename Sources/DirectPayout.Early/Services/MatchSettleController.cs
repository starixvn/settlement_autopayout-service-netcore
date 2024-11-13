using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;

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
            Console.WriteLine($"Process {match.MatchId}");
            await validationService.VerifyRelatedResults(match);
            Console.WriteLine("Done VerifyRelatedResults");
            await validationService.VerifyNoRunningOdds(match);
            Console.WriteLine("Done VerifyNoRunningOdds");

            var settleCriteria = CreateSettleCriteria(match);
            await validationService.VerifyEventAndTicket(settleCriteria);
            Console.WriteLine("Done VerifyEventAndTicket");

            await settleService.Settle(match, settleCriteria);
            Console.WriteLine("Done Settle");
        }
        catch (AppException exception) when (exception is not IgnoreSettlementException)
        {
            Console.WriteLine(exception);
            await HandleAutoPayoutException(match, exception);
        }
        catch (IgnoreSettlementException exception)
        {
            Console.WriteLine(exception);
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
        var errorMessage = $"Failed Direct Payout: Result ({string.Join(",", match.ResultDetails.Select(x => x.ToString()))}) {exception.Message}";

        await autoPayoutResultService.UpdAutoPayoutFailResult(match, errorMessage);
        await logSettleService.WriteLogErrorSettle(match, errorMessage);
    }

    private async Task ExceptionLog(Match match, Exception exception)
    {
        var logMessage = new LogMessage($"Match: {match.ToString()} - {exception.Message}")
            .ToString();

        await LogService.ErrorAsync(logMessage);
    }
}