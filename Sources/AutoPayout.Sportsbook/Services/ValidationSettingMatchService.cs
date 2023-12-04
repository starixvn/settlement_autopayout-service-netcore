using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services;

public interface ValidationSettingMatchService
{
    void VerifySettingMatch(RemarkSettingModel remark);
}

public class ValidationSettingMatchServiceImp : ValidationSettingMatchService
{
    public void VerifySettingMatch(RemarkSettingModel remark)
    {
        bool[] conditionsToCheck = { remark.IsMtdHold, remark.IsUndecided, remark.IsPending, remark.IsUnderSettle, remark.IsHoldKey };

        if (Array.Exists(conditionsToCheck, item => item))
        {
            throw new RemarkException();
        }
    }
}