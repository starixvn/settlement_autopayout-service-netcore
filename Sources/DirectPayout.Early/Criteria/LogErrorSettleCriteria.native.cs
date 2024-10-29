namespace DirectPayout.Early.Criteria;

public class LogErrorSettleCriteria : NonQueryCommand
{
    public int MatchId { get; set; }
    public string Remark { get; set; }
    public int ActionId { get; set; }
    public override string GetSettingKey() => "WriteLog_UserLog_Settle";
    public override bool IsValid() => MatchId > 0;
}
