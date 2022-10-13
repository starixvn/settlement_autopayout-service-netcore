using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services;

public interface ValidationStatusMatchService
{
    void VerifyStatusMatch(Match match);
}

public class ValidationStatusMatchServiceImp : ValidationStatusMatchService
{
    private const string RefundHalftime = "Refund HT";

    public void VerifyStatusMatch(Match match)
    {
        var isMatch3X30 = IsMatch3X30(match);
        if (isMatch3X30)
        {
            throw new Match3X30Exception();
        }

        var isHalfTimeNotDone = IsHalfTimeNotDone(match);
        if (isHalfTimeNotDone)
        {
            throw new HalftimeNotDoneException();
        }
    }

    private static bool IsMatch3X30(Match match)
    {
        return match.IsFullTimeConfirmed && !string.IsNullOrWhiteSpace(match.BetStatus) && match.BetStatus.Contains(RefundHalftime);
    }

    private static bool IsHalfTimeNotDone(Match match)
    {
        return match.TypeId == SettleType.FullTime && !match.IsHalfTimeCalculated && !match.IsSabaPenaltyShootout;
    }
}