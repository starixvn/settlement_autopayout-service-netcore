namespace AutoPayout.Sportsbook.Services;

using Exceptions;
using Models;

public interface ValidationSettingMatchService
{
    void VerifySettingMatch(RemarkSettingModel remark);
}

public class ValidationSettingMatchServiceImp : ValidationSettingMatchService
{
    public void VerifySettingMatch(RemarkSettingModel remark)
    {
        bool[] conditionsToCheck = [remark.IsMtdHold, remark.IsUndecided, remark.IsPending, remark.IsUnderSettle, remark.IsHoldKey];

        if (Array.Exists(conditionsToCheck, item => item))
        {
            throw new RemarkException();
        }
    }
}