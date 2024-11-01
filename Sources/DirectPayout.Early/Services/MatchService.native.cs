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
    private const string KeyLeagueId = "Settlement-AutoPayout-AutoInstant-LeagueId";

    public async Task<List<Match>> GetMatches()
    {
        var matches = await GetWaitingMatches();

        return await GetMatchesByEnv(matches);
    }

    private async Task<List<Match>> GetMatchesByEnv(List<Match> matches)
    {
        var (_, response) = await cache.TryGetAsync<string>(KeyLeagueId);
        var leagueIds = (response ?? "").Split(",");

        return appSettingsKey.Value.EnvironmentName switch
        {
            "UAT" => matches.Where(m => leagueIds.Contains(m.LeagueId.ToString())).ToList(),
            "PRO" => leagueIds.Length > 0 ? matches.Where(m => !leagueIds.Contains(m.LeagueId.ToString())).ToList() : matches,
            _ => matches
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