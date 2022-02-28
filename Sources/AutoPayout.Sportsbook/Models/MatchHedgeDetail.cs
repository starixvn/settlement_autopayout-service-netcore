namespace AutoPayout.Sportsbook.Models
{
    public class MatchHedgeDetail
    {
        public int MatchId { get; set; }

        public string MatchCode { get; set; }

        public bool IsDeterminedHedgeMatch { get; set; }

        public bool IsUndecided { get; set; }
    }
}