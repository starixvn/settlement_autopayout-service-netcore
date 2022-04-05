namespace AutoPayout.Result.Models
{
    public class Goal
    {
        public bool IsExistHtFgLg { get; set; }

        public bool IsExistFgLg { get; set; }

        public bool IsExistFgNg { get; set; }

        public bool IsEnabledGoalRecordHt { get; set; }

        public bool IsEnabledGoalRecordFt { get; set; }

        public string GoalRecord { get; set; }
    }
}