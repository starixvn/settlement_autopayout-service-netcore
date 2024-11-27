using Fanex.Caching;

namespace AutoPayout.Sportsbook.Services;

using AutoPayout.Core.Constants;
using Models;

public interface MatchByLeagueService
{
    Task<List<Match>> GetMatches();
}

public class MatchByLeagueServiceImp(
    MatchQueryService matchQueryService,
    ICache cache,
    IOptions<AppSettingsKey> appSettingsKey)
    : MatchByLeagueService
{
    public async Task<List<Match>> GetMatches()
    {
        var matches = await matchQueryService.GetWaitingMatches();

        var (_, leagueId) = await cache.TryGetAsync<int>(CacheKey.LeagueId);

        return appSettingsKey.Value.EnvironmentName switch
        {
            "PRO" => leagueId > 0
                ? (from m in matches where m.LeagueId != leagueId select m).ToList()
                : matches,
            "UAT" => (from m in matches where m.LeagueId == leagueId select m).ToList(),
            _ => matches
        };
    }
}