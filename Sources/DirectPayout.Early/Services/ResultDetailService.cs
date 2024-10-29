using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface ResultDetailService
{
    Task<IEnumerable<ResultDetail>> GetResultDetailsAsync();
}

public class ResultDetailServiceImp : ResultDetailService
{
    private readonly IDynamicRepository repository;

    public ResultDetailServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }


    public async Task<IEnumerable<ResultDetail>> GetResultDetailsAsync()
    {
        var resultDetail = await repository.FetchAsync<ResultDetail>(new ScheduleSelCriteria());

        var validResults = resultDetail.Where(r => r.LastModifyDate <= DateTime.Now);

        return validResults;
    }
}
