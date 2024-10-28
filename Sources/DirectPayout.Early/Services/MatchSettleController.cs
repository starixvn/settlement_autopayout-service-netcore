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

    public MatchSettleControllerImp(
        MatchSettlingPoolService matchSettlingPoolService,
        ValidationService validationService,
        XmlConvertService xmlConvertService,
        SettleService settleService,
        AutoPayoutResultService autoPayoutResultService)
    {
        this.matchSettlingPoolService = matchSettlingPoolService;
        this.validationService = validationService;
        this.xmlConvertService = xmlConvertService;
        this.settleService = settleService;
        this.autoPayoutResultService = autoPayoutResultService;
    }

    public async Task Process(Match match)
    {
        try
        {
            await validationService.VerifyNoRunningOdds(match);

            var settleCriteria = CreateSettleCriteria(match);
            await validationService.VerifyEventAndTicket(settleCriteria);

            await settleService.Settle(settleCriteria);
            //log
            //insert

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
        var resources = new Resources<ResultDetail>(match.ResultDetails);

        return new SettleCriteria
        {
            MatchId = match.MatchId,
            SelectedResources = xmlConvertService.Serialize(resources),
        };
    }

    private async Task HandleAutoPayoutException(Match match, AutoPayoutException exception)
    {
        var settleCriteria = CreateSettleCriteria(match);
        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Failed, exception.Message);

    }
}