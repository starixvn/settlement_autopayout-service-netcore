using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;

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

    public ValidationServiceImp(IDynamicRepository repository, XmlConvertService xmlConvertService, ScaleOutSettleService scaleOutSettleService)
    {
        this.repository = repository;
        this.xmlConvertService = xmlConvertService;
        this.scaleOutSettleService = scaleOutSettleService;
    }

    public async Task VerifyEventAndTicket(SettleCriteria criteria)
    {
        var request = new EventTicketRequest
        {
            MatchId = criteria.MatchId,
            Action = criteria.Action, // "CAL"
            SpName = criteria.SpName,
            Mode = "EAR",
            SelectedResources = criteria.SelectedResources
        };

        var resultMessage = await scaleOutSettleService.ValidateEventAndTicket(request);

        if (resultMessage.ErrorCode != 0)
        {
            throw new AppException(resultMessage.ErrorMessage);
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

        var resources = new Resources<CheckOddsItem>(new List<CheckOddsItem> { checkOddsItem });

        return xmlConvertService.Serialize(resources);
    }

}
