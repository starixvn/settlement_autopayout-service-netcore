namespace AutoPayout.Sportsbook.Criteria;
public class LogDirectlyCriteria : NonQueryCommand
{
    public int UserID { get; set; }
    public string Doing { get; set; }
    public int Extend { get; set; }

    public override string GetSettingKey() => "NAP_UserLog_WriteLogaaaa";
    public override bool IsValid() => true;
}
