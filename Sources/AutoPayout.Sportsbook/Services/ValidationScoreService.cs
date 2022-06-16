using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using System.Collections.Generic;
using System.Linq;

namespace AutoPayout.Sportsbook.Services
{
    public interface ValidationScoreService
    {
        void VerifyScore(Match match, Score score);

        void VerifyResultsConsistent(Match match, List<ResultDetail> results);
    }

    public class ValidationScoreServiceImp : ValidationScoreService
    {
        private readonly OtherInfoService otherInfoService;

        public ValidationScoreServiceImp(OtherInfoService otherInfoService)
        {
            this.otherInfoService = otherInfoService;
        }

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

        public void VerifyResultsConsistent(Match match, List<ResultDetail> results)
        {
            var isRelatedMatchesCalculated = results.All(result => result.ResultStatusId == ResultStatus.ManualCompleted || result.ResultStatusId == ResultStatus.AutoCompleted);
            if (!isRelatedMatchesCalculated)
            {
                var remark = match.TypeId == SettleType.Halftime ? "HT" : "FT";
                throw new RelatedMatchesNotCalculatedException(remark);
            }

            if (IsMatchScoreInconsistentWithRelatedMatches(match, results))
            {
                var type = match.TypeId == SettleType.Halftime ? "HT" : "FT";
                var (mainScore, cornerScore) = otherInfoService.GetRelatedMatchesScore(results);
                throw new RelatedMatchesInconsistentException(type, mainScore, cornerScore);
            }
        }

        private bool IsMatchScoreInconsistentWithRelatedMatches(Match match, List<ResultDetail> results)
        {
            var totalHomeScore = results.Sum(result => result.HomeValue);
            var totalAwayScore = results.Sum(result => result.AwayValue);

            var homeScoreToCompare = match.TypeId == SettleType.Halftime ? match.HtHomeScore : match.FinalHomeScore;
            var awayScoreToCompare = match.TypeId == SettleType.Halftime ? match.HtAwayScore : match.FinalAwayScore;

            return totalHomeScore != homeScoreToCompare || totalAwayScore != awayScoreToCompare;
        }
    }
}