using AutoPayout.Sportsbook.Constants;

namespace AutoPayout.Sportsbook.Services;

using Exceptions;
using Models;
using System.Threading.Tasks;

public interface ValidationSettingMatchService
{
    void VerifySettingMatch(RemarkSettingModel remark);

    Task VerifyHoldResult(Match match);
}

public class ValidationSettingMatchServiceImp(ScoreService scoreService) : ValidationSettingMatchService
{
    private readonly string ResourceHT = "HT";

    private readonly string ResourceFT = "FT";

    public void VerifySettingMatch(RemarkSettingModel remark)
    {
        bool[] conditionsToCheck = [remark.IsMtdHold, remark.IsUndecided, remark.IsPending, remark.IsUnderSettle, remark.IsHoldKey];

        if (Array.Exists(conditionsToCheck, item => item))
        {
            throw new RemarkException();
        }
    }

    public async Task VerifyHoldResult(Match match)
    {
        var resultDetails = await scoreService.GetResultDetail(match, true);
        var resultMainMatch = resultDetails.Where(r => r.MatchId == match.MatchId).ToList();
        if (match.TypeId == SettleType.Halftime)
        {
            resultMainMatch = resultMainMatch.Where(r => r.ResourceId == ResourceId.HalfTime).ToList();
        }

        if (resultMainMatch.Exists(r => r.IsHold))
        {
            throw new HoldResultException(string.Join(" or ",
                resultMainMatch.Select(r => r.ResourceId switch
                {
                    ResourceId.HalfTime => ResourceHT,
                    ResourceId.FullTime => ResourceFT,
                    _ => r.ResourceId
                })));
        }
    }
}