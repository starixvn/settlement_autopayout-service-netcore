using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using System.Threading.Tasks;

namespace AutoPayout.Sportsbook.Services
{
    public interface ScoreMatcherController
    {
        Task<int> Verify(Match match);
    }

    public class ScoreMatcherControllerImp : ScoreMatcherController
    {
        private readonly ValidationSettingMatchService validationSettingMatchService;

        private readonly ValidationStatusMatchService validationStatusMatchService;

        private readonly ValidationScoreService validationScoreService;

        private readonly ScoreService scoreService;

        private readonly ValidationGoalRecordService validationGoalRecordService;

        private readonly MatchQueryService matchQueryService;

        public ScoreMatcherControllerImp(
            ValidationSettingMatchService validationSettingMatchService,
            ValidationStatusMatchService validationStatusMatchService,
            ValidationScoreService validationScoreService,
            ScoreService scoreService,
            ValidationGoalRecordService validationGoalRecordService,
            MatchQueryService matchQueryService)
        {
            this.validationSettingMatchService = validationSettingMatchService;
            this.validationStatusMatchService = validationStatusMatchService;
            this.validationScoreService = validationScoreService;
            this.scoreService = scoreService;
            this.validationGoalRecordService = validationGoalRecordService;
            this.matchQueryService = matchQueryService;
        }

        public async Task<int> Verify(Match match)
        {
            validationSettingMatchService.VerifySettingMatch(match.Remark);

            validationStatusMatchService.VerifyStatusMatch(match);

            if (match.IsDirectSettlement)
            {
                validationGoalRecordService.VerifyGoalRecord(match);
                return Source.AdminSite;
            }

            return await GetSourceId(match);
        }

        private async Task<int> GetSourceId(Match match)
        {
            var sourceId = Source.AdminSite;
            if (match.IsTotalCornerGoalMatch)
            {
                var results = await scoreService.TryGetRelatedResults(match);
                validationScoreService.VerifyResultsConsistent(match, results);
            }
            else
            {
                var score = await scoreService.TryGetScore(match);
                sourceId = score.SourceId;
                validationScoreService.VerifyScore(match, score);
                validationGoalRecordService.VerifyGoalRecord(match);
            }

            var (matchRemark, _) = await matchQueryService.GetMatchAndResultDetail(match.MatchId);
            validationSettingMatchService.VerifySettingMatch(matchRemark);

            return sourceId;
        }
    }
}