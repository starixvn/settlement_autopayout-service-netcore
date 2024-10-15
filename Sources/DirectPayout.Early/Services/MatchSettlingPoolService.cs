using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchSettlingPoolService
{
    Task<List<Match>> PeekNewMatch();

    void Remove(Match match);
}

public class MatchSettlingPoolServiceImp(MatchByLeagueService matchByLeagueService) : MatchSettlingPoolService
{
    private readonly List<Match> poolOfMatches = [];

    private readonly object lockObject = new();

    public List<Match> PoolOfMatches => poolOfMatches;

    public void Remove(Match match)
    {
        lock (lockObject)
        {
            poolOfMatches.RemoveAll(m => m.MatchId == match.MatchId && m.ResourceId == match.ResourceId);
        }
    }

    public async Task<List<Match>> PeekNewMatch()
    {
        var matches = await matchByLeagueService.GetMatches();

        lock (lockObject)
        {
            var newMatches = matches.Except(poolOfMatches).ToList();
            poolOfMatches.AddRange(newMatches);

            return newMatches;
        }
    }
}