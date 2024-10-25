using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using Fanex.Settlement.Converter.Services;

namespace DirectPayout.Early.Services;

public class VerificationService
{
    private readonly IDynamicRepository repository;
    private readonly XmlConvertService xmlConvertService;

    public VerificationService(IDynamicRepository repository, XmlConvertService xmlConvertService)
    {
        this.repository = repository;
        this.xmlConvertService = xmlConvertService;
    }

    public async Task VerifyNoRunningOdds(Match match)
    {
        var selectedResource = CreateMatchResources(match);

        var criteria = new VerifyNoRunningOddsCriteria
        {
            MatchId = match.MatchId,
            SelectedResource = selectedResource,
            Action = "CAL"
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
