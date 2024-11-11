using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface AutoPayoutResultService
{
    Task InsOrUpdAutoPayoutResult(Match match, int statusId, string remark = null);

    Task UpdAutoPayoutFailResult(Match match, string remark);
}

public class AutoPayoutResultServiceImp : AutoPayoutResultService
{
    private readonly IDynamicRepository repository;
    private readonly XmlConvertService xmlConvertService;

    public AutoPayoutResultServiceImp(IDynamicRepository repository, XmlConvertService xmlConvertService)
    {
        this.repository = repository;
        this.xmlConvertService = xmlConvertService;
    }

    public async Task InsOrUpdAutoPayoutResult(Match match, int statusId, string remark = null)
    {
        var resources = new Resources<ResultDetail>
        {
            ResourceItems = match.ResultDetails
        };

        var criteria = new AutoPayoutResultInsOrUpdCriteria
        {
            MatchId = match.MatchId,
            StatusId = statusId,
            SelectedResources = xmlConvertService.Serialize(resources),
            Remark = remark
        };
        await repository.ExecuteAsync(criteria);
    }

    public async Task UpdAutoPayoutFailResult(Match match, string remark)
    {
        if (match.ResultDetails.Any())
        {
            await InsOrUpdAutoPayoutResult(match, AutoPayoutResultStatus.Failed, remark);
        }
    }
}
