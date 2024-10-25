using DirectPayout.Early.Criteria;

namespace DirectPayout.Early.Services;

public class AutoPayoutResultService
{
    private readonly IDynamicRepository repository;

    public AutoPayoutResultService(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task InsertAutoPayoutResult()
    {
        var criteria = new AutoPayoutResultInsCriteria();
        await repository.ExecuteAsync(criteria);
    }

    public async Task UpdateAutoPayoutResult()
    {
        var criteria = new AutoPayoutResultUpdCriteria();
        await repository.ExecuteAsync(criteria);
    }
}
