using AutoPayout.Core.Models;
using AutoPayout.Sportsbook.Exceptions;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Settlement.Converter.Services;

namespace DirectPayout.Early.Services;

public interface MatchSettleController
{
    Task Process(Match match);
}

public class MatchSettleControllerImp : MatchSettleController
{
    private readonly MatchSettlingPoolService matchSettlingPoolService;
    private readonly ValidationService validationService;
    private readonly XmlConvertService xmlConvertService;
    private readonly SettleService settleService;
    private readonly AutoPayoutResultService autoPayoutResultService;
    private readonly LogSettleService logSettleService;

    public MatchSettleControllerImp(
        MatchSettlingPoolService matchSettlingPoolService,
        ValidationService validationService,
        XmlConvertService xmlConvertService,
        SettleService settleService,
        AutoPayoutResultService autoPayoutResultService,
        LogSettleService logSettleService)
    {
        this.matchSettlingPoolService = matchSettlingPoolService;
        this.validationService = validationService;
        this.xmlConvertService = xmlConvertService;
        this.settleService = settleService;
        this.autoPayoutResultService = autoPayoutResultService;
        this.logSettleService = logSettleService;
    }

    public async Task Process(Match match)
    {
        try
        {
            await validationService.VerifyNoRunningOdds(match);

            var settleCriteria = CreateSettleCriteria(match);
            await validationService.VerifyEventAndTicket(settleCriteria);

            await settleService.Settle(match, settleCriteria);
        }
        catch (AutoPayoutException exception)
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
        var resources = new Resources<ResultDetail>()
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

    private async Task HandleAutoPayoutException(Match match, AutoPayoutException exception)
    {
        var settleCriteria = CreateSettleCriteria(match);
        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Failed, exception.Message);
        await logSettleService.WriteLogErrorSettle(match, exception.Message);
    }
}