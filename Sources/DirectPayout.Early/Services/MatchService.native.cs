using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Caching;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface MatchService
{
    Task<List<Match>> GetMatches();
}

public class MatchServiceImp : MatchService
{
    private const string KeyLeagueId = "Settlement-AutoPayout-AutoInstant-LeagueId";

    private readonly ResultDetailService resultDetailService;
    private readonly ICache cache;
    private readonly AppSettingsKey appSettingsKey;
    private readonly IDynamicRepository repository;

    public MatchServiceImp(ResultDetailService resultDetailService, ICache cache, IOptions<AppSettingsKey> appSettingsKey)
    {
        this.resultDetailService = resultDetailService;
        this.cache = cache;
        this.appSettingsKey = appSettingsKey.Value;
    }

    public async Task<List<Match>> GetMatches()
    {
        var matches = await GetWaitingMatches();

        return await GetMatchesByEnv(matches);
    }

    public async Task<MatchDetail> GetMatchDetail(int matchId, short sportType)
    {
        var matchDetail = await repository.GetAsync<MatchDetail>(new MatchDetailSelCriteria { MatchId = matchId });

        return matchDetail;
    }



    private async Task<List<Match>> GetMatchesByEnv(List<Match> matches)
    {
        var (_, response) = await cache.TryGetAsync<string>(KeyLeagueId);
        var leagueIds = (response ?? "").Split(",");

        return appSettingsKey.EnvironmentName switch
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