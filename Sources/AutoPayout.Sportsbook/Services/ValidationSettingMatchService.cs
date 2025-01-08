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
    private const string ResourceHT = "HT";

    private const string ResourceFT = "FT";

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

        if (match.TypeId == SettleType.Halftime)
        {
            resultDetails = resultDetails.Where(r => r.ResourceId == ResourceId.HalfTime).ToList();
        }

        if (resultDetails.Exists(r => r.IsHold))
        {
            throw new HoldResultException(string.Join("or",
                resultDetails.Select(r => r.ResourceId switch
                {
                    ResourceId.HalfTime => ResourceHT,
                    ResourceId.FullTime => ResourceFT,
                    _ => r.ResourceId
                })));
        }
    }
}