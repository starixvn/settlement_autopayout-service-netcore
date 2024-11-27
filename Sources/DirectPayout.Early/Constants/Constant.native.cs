
namespace DirectPayout.Early.Constants;

public static class ErrorMsg
{
    public const string RunningOdds = "ERROR! Cannot calculate due to unclosed odds. Please make sure you select correct match/interval/result to calculate.";
    public const string MatchCalculated = "This match is being calculated, please wait a few seconds until the on-going process is completed and try again.";
    public const string InvalidInput = "WARNING! Cannot continue due to invalid value inputted. Please check and try again.";
    public const string FTInconsistent = "Failed Direct Payout: FT is inconsistent with intervals or OT.";
    public const string ExpiredResults = "Failed Direct Payout: {0} not been settled related results accordingly.";
    public const string MatchNotValidResults = "Failed Direct Payout: Match hasn't valid result settle";

}

public static class ErrorCodes
{
    public const string RunningOdds = "DP-ERR1";
    public const string MatchCalculated = "DP-ERR2";
    public const string InvalidInput = "DP-ERR3";
    public const string FTInconsistent = "DP-ERR4";
    public const int MatchInQueue = 1_001_003;
}

public static class SettlementStatusId
{
    public const int Calculated = 4;

    public const int Running = 0;
}

public static class ResultStatusId
{
    public const int Running = 1;

    public const int AutoCompleted = 2;

    public const int ManualCompleted = 5;
}
