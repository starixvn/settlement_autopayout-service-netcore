using DirectPayout.Early.Criteria;

namespace DirectPayout.Early.Services;

public interface AutoPayoutResultService
{
    Task InsOrUpdAutoPayoutResult(SettleCriteria settleCriteria, int statusId, string remark = null);
}

public class AutoPayoutResultServiceImp : AutoPayoutResultService
{
    private readonly IDynamicRepository repository;

    public AutoPayoutResultServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task InsOrUpdAutoPayoutResult(SettleCriteria settleCriteria, int statusId, string remark = null)
    {
        var criteria = new AutoPayoutResultInsOrUpdCriteria
        {
            MatchId = settleCriteria.MatchId,
            StatusId = statusId,
            SelectedResources = settleCriteria.SelectedResources,
            Remark = remark
        };
        await repository.ExecuteAsync(criteria);
    }
}
