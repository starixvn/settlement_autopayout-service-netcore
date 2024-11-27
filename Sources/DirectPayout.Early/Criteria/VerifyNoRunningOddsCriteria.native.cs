namespace DirectPayout.Early.Criteria;

public class VerifyNoRunningOddsCriteria : NonQueryCommand
{
    public int MatchId { get; set; }

    public string SelectedResource { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public byte CheckType { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; } = string.Empty;

    public override string GetSettingKey() => "NAP_Admin_Settlement_Validation_PrePush";

    public override bool IsValid() => MatchId > 0;
}