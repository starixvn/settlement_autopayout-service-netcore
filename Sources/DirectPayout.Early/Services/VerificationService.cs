using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Settlement.Converter.Services;

namespace DirectPayout.Early.Services;

public interface IVerificationService
{
    Task VerifyNoRunningOdds(Match match);
}

public class VerificationServiceImp(IDynamicRepository repository, XmlConvertService xmlConvertService)
    : IVerificationService
{
    private const int iceHockeySet = 4;

    private const int OTIceHockeyIndex = 4;

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

        var resources = new Resources<CheckOddsItem>
        {
            ResourceItems = new List<CheckOddsItem> { checkOddsItem }
        };

        return xmlConvertService.Serialize(resources);
    }
}