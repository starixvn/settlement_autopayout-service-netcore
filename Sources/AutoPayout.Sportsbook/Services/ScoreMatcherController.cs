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
            else
            {
                var score = await scoreService.TryGetScore(match);
                validationScoreService.VerifyScore(match, score);
                validationGoalRecordService.VerifyGoalRecord(match);
                var matchRemark = await matchQueryService.GetMatchRemark(match.MatchId);
                validationSettingMatchService.VerifySettingMatch(matchRemark);

                return score.SourceId;
            }
        }
    }
}