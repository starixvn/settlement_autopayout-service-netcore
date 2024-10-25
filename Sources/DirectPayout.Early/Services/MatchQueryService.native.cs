using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchQueryService
{
    Task<List<Match>> GetWaitingMatches();
}

public class MatchQueryServiceImp(IDynamicRepository repository)
    : MatchQueryService
{
    public async Task<List<Match>> GetWaitingMatches()
    {
        var result = await repository.FetchAsync<Match>(new ScheduleSelCriteria());
        return result.ToList();
    }
}