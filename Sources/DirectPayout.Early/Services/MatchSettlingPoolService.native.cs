using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchSettlingPoolService
{
    Task<List<Match>> PeekNewMatch();

    void Remove(Match match);
}

public class MatchSettlingPoolServiceImp(MatchService matchService) : MatchSettlingPoolService
{
    private readonly Lock lockObject = new();

    private List<Match> PoolOfMatches { get; } = [];

    public void Remove(Match match)
    {
        lock (lockObject)
        {
            PoolOfMatches.RemoveAll(m => m.MatchId == match.MatchId);
        }
    }

    public async Task<List<Match>> PeekNewMatch()
    {
        var matches = await matchService.GetMatches();

        lock (lockObject)
        {
            var newMatches = matches.Except(PoolOfMatches).ToList();
            PoolOfMatches.AddRange(newMatches);

            return newMatches;
        }
    }
}