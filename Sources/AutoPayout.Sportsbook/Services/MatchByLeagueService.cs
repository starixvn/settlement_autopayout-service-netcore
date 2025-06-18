using Fanex.Caching;

namespace AutoPayout.Sportsbook.Services;

using AutoPayout.Core.Constants;
using AutoPayout.Sportsbook.Constants;
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
        foreach (var item in matches)
        {
            item.MatchType = GetMatchType(item);
        }

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

    private byte GetMatchType(Match match)
    {
        if (match.ChildMatchType == MatchTypes.PenaltyShootoutHdp ||
            match.ChildMatchType == MatchTypes.PenaltyShootoutOu)
            return MatchTypes.PenaltyShootout;

        if (match.ChildMatchType == MatchTypes.ExtraTime)
            return MatchTypes.ExtraTime;

        return (match.ChildLeagueType, match.MainType, match.SubType) switch
        {
            (1018, 19, 6200) => MatchTypes.TotalGoalsMinutes,
            (1016, 17, 900) => MatchTypes.InjuryTimeFirstHalf,
            (1017, 18, 900) => MatchTypes.InjuryTimeSecondHalf,
            _ => 0
        };
    }
}