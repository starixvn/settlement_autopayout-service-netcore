using AutoPayout.Core.Constants;
using AutoPayout.Core.Models;
using DirectPayout.Early.Models;
using Fanex.Caching;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface MatchService
{
    Task<List<Match>> GetMatches();
}

public class MatchServiceImp(
    ResultDetailService resultDetailService,
    ICache cache,
    IOptions<AppSettingsKey> appSettingsKey)
    : MatchService
{
    public async Task<List<Match>> GetMatches()
    {
        var matches = await GetWaitingMatches();

        return await GetMatchesByEnv(matches);
    }

    private async Task<List<Match>> GetMatchesByEnv(List<Match> matches)
    {
        var (_, leagueId) = await cache.TryGetAsync<int>(CacheKey.LeagueId);
        return appSettingsKey.Value.EnvironmentName switch
        {
            "PRO" => leagueId > 0
                                ? (from m in matches where m.LeagueId != leagueId select m).ToList()
                                : matches,
            "UAT" => (from m in matches where m.LeagueId == leagueId select m).ToList(),
            _ => matches,
        };
    }

    private async Task<List<Match>> GetWaitingMatches()
    {
        var results = await resultDetailService.GetResultDetailsAsync();

        var matches = results.GroupBy(x => new { x.MatchId, x.LeagueId })
             .Select(x => new Match
             {
                 MatchId = x.Key.MatchId,
                 LeagueId = x.Key.LeagueId,
                 ResultDetails = x.ToList()
             }).ToList();

        return matches;
    }
}