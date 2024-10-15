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
        var currentDate = DateTime.Now;
        return new List<Match>
        {
            new Match
            {
                MatchId = 83688898,
                LeagueId = 1,
                AwayValue = "",
                HomeValue = "NG",
                ResourceId = 424,
                WaitingTime = 3,
                LastModifyDate = currentDate.AddMinutes(-2)
            },
            new Match
            {
                MatchId =83688898,
                LeagueId = 1,
                AwayValue = "",
                HomeValue = "81-90",
                ResourceId = 455,
                WaitingTime = 3,
                LastModifyDate = currentDate.AddMinutes(-2)
            }
        };
        //var criteria = new ScheduleSelCriteria();
        //return (await repository.FetchAsync<Match>(criteria)).ToList();
    }
}