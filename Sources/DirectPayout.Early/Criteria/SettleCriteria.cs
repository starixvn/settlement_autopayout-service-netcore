using Fanex.Data;

namespace DirectPayout.Early.Criteria;

public class SettleCriteria : NonQueryCommand
{
    public int AdminId { get; set; }

    public int MatchId { get; set; }

    public string SelectedResources { get; set; }

    public string Action { get; set; } = "CAL";
    public bool IsAuto { get; set; } = true;

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    [SpParam(Ignored = true)]
    public string SpName => "NAP_Admin_Settlement_Auto_1s_Push";

    public override string GetSettingKey() => $"NAP_Admin_Settlement_Auto_1s_Push_{DbServer}";

    public override bool IsValid() => AdminId > 0 && MatchId > 0;
}