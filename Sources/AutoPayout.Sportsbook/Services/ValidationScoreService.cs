using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services
{
    public interface ValidationScoreService
    {
        void VerifyScore(Match match, Score score);
    }

    public class ValidationScoreServiceImp : ValidationScoreService
    {
        public void VerifyScore(Match match, Score score)
        {
            if (score.MatchId < 1)
            {
                throw new SourceNotFoundException();
            }

            if (!IsMatchedScore(match, score))
            {
                throw new ScoreMismatchedException();
            }
        }

        private static bool IsMatchedScore(Match match, Score score)
        {
            return match.TypeId == SettleType.Halftime
                ? IsCheckScore(match.HtHomeScore.ToString(), match.HtAwayScore.ToString(), score)
                : IsCheckScore(match.FinalHomeScore.ToString(), match.FinalAwayScore.ToString(), score);
        }

        private static bool IsCheckScore(string home, string away, Score score)
        {
            return home == score.HomeValue && away == score.AwayValue;
        }
    }
}