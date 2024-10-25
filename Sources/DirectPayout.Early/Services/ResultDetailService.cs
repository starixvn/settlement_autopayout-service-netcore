using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public class ResultDetailService
{
    private readonly IDynamicRepository repository;

    public ResultDetailService(IDynamicRepository repository)
    {
        this.repository = repository;
    }


    public async Task<IEnumerable<ResultDetail>> GetResultDetailsAsync()
    {
        var resultDetail = await repository.FetchAsync<ResultDetail>(new ScheduleSelCriteria());
        return resultDetail;
    }
}
