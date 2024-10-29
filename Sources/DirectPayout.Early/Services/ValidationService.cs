using AutoPayout.Core.Models;
using DirectPayout.Early.Constants;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut.Models;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.Extensions.Options;
using Polly;

namespace DirectPayout.Early.Services;

public interface ValidationService
{
    Task VerifyNoRunningOdds(Match match);
    Task VerifyEventAndTicket(SettleCriteria criteria);
}

public class ValidationServiceImp : ValidationService
{
    private readonly IDynamicRepository repository;
    private readonly XmlConvertService xmlConvertService;
    private readonly ScaleOutSettleService scaleOutSettleService;
    private readonly AppSettingsKey appSettings;

    public ValidationServiceImp(
        IDynamicRepository repository,
        XmlConvertService xmlConvertService,
        ScaleOutSettleService scaleOutSettleService,
        IOptions<AppSettingsKey> options)
    {
        this.repository = repository;
        this.xmlConvertService = xmlConvertService;
        this.scaleOutSettleService = scaleOutSettleService;
        this.appSettings = options.Value;
    }

    public async Task VerifyEventAndTicket(SettleCriteria criteria)
    {
        var request = new EventTicketRequest
        {
            MatchId = criteria.MatchId,
            Action = criteria.Action, // "CAL"
            SpName = criteria.SpName,
            Mode = "EAR",
            IsAuto = true,
            SelectedResources = criteria.SelectedResources
        };

        var result = await Policy
            .HandleResult<ResultMessage>(r => r.ErrorCode == ErrorCodes.MatchInQueue)
            .WaitAndRetryAsync(appSettings.RetryCount, retryAttempt => TimeSpan.FromSeconds(appSettings.RetryInterval))
            .ExecuteAsync(async () => await scaleOutSettleService.ValidateEventAndTicket(request));


        if (result.ErrorCode != 0)
        {
            throw new AppException(result.ErrorMessage);
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


    private string CreateMatchResources(Match match)
    {
        var resourceIds = match.ResultDetails.Select(resultDetail => resultDetail.ResourceId).ToList();

        var checkOddsItem = new CheckOddsItem
        {
            MatchId = match.MatchId,
            ResourceIds = resourceIds
        };

        var resources = new Resources<CheckOddsItem>()
        {
            ResourceItems = new List<CheckOddsItem> { checkOddsItem }
        };

        return xmlConvertService.Serialize(resources);
    }

}
