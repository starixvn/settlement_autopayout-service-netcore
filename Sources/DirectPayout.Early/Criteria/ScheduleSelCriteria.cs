namespace DirectPayout.Early.Criteria;

public class ScheduleSelCriteria : CriteriaBase
{
    public override bool IsValid() => true;

    public override string GetSettingKey() => "NAP_Set_AutoPayout_Resource_ScheduleSel";
}