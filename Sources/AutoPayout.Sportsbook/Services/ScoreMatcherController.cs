namespace AutoPayout.Sportsbook.Services;

using Constants;
using Models;

public interface ScoreMatcherController
{
    Task<int> Verify(Match match);
}

public class ScoreMatcherControllerImp(
    ValidationSettingMatchService validationSettingMatchService,
    ValidationStatusMatchService validationStatusMatchService,
    ValidationScoreService validationScoreService,
    ScoreService scoreService,
    ValidationGoalRecordService validationGoalRecordService,
    MatchQueryService matchQueryService)
    : ScoreMatcherController
{
    public async Task<int> Verify(Match match)
    {
        validationSettingMatchService.VerifySettingMatch(match.Remark);
        await validationSettingMatchService.VerifyHoldResult(match);
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
        await validationSettingMatchService.VerifyHoldResult(match);

        return sourceId;
    }
}