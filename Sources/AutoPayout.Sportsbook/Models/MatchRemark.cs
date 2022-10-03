using Kaden.Extensions;

namespace AutoPayout.Sportsbook.Models;

public class MatchRemark
{
    private const int ToBase = 2;

    public byte SettlementStatus { get; set; }

    public RemarkSettingModel Remark => new RemarkSettingModel
    {
        SettlementStatus = Convert.ToString(SettlementStatus, ToBase).Reverse(),
    };
}