namespace AutoPayout.Sportsbook.Models;

public class ResultDetail
{
    private const int ToBase = 2;

    private const int HoldResultPosition = 0;


    public int MatchId { get; set; }
    public string ResourceId { get; set; }

    public string HomeValue { get; set; }

    public string AwayValue { get; set; }

    public int ChildMatchType { get; set; }

    public byte SettlementStatusId { get; set; }

    public byte ResultSetting { get; set; }

    public bool IsHold
    {
        get
        {
            var value = Convert.ToString(ResultSetting, ToBase).Reverse();
            return !string.IsNullOrWhiteSpace(value) && value.Length > HoldResultPosition && value[HoldResultPosition].Equals('1');
        }
    }

}
