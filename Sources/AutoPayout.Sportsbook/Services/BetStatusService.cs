using Kaden.Extensions;

namespace AutoPayout.Sportsbook.Services;

public interface BetStatusService
{
    string GenerateBetStatus(string goalRecord);
    string GenerateBetStatus(string prefix, string goalRecord);
}

public class BetStatusServiceImp : BetStatusService
{
    private const string HomeGoal = "1";

    private const string AwayGoal = "2";

    public string GenerateBetStatus(string goalRecord)
    {
        if (string.IsNullOrWhiteSpace(goalRecord))
        {
            return string.Empty;
        }
        else
        {
            var firstGoal = goalRecord.IgnoreCaseStartsWith("h") ? HomeGoal : AwayGoal;

            return BetStatus("FL", firstGoal, "0");
        }
    }

    public string GenerateBetStatus(string prefix, string goalRecord)
    {
        if (string.IsNullOrWhiteSpace(goalRecord))
        {
            return BetStatus(prefix, "0", "0");
        }
        else
        {
            var firstGoal = goalRecord.IgnoreCaseStartsWith("h") ? HomeGoal : AwayGoal;
            var lastGoal = goalRecord.IgnoreCaseEndsWith("h") ? HomeGoal : AwayGoal;

            return BetStatus(prefix, firstGoal, lastGoal);
        }
    }

    private static string BetStatus(string prefix, string firstGoal, string lastGoal)
    {
        return $"{prefix}:{firstGoal}-{lastGoal};";
    }
}