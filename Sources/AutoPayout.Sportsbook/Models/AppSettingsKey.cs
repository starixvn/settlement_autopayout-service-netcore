namespace AutoPayout.Sportsbook.Models
{
    public class AppSettingsKey
    {
        public int RetryCount { get; set; }

        public int RetryInterval { get; set; }

        public string EnvironmentName { get; set; }

        public int TotalGoalCornerInterval { get; set; }

        public int TotalGoalCornerRetryCount { get; set; }
    }
}
