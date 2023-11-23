using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services;

public interface OtherInfoService
{
    (string mainScore, string cornerScore) GetRelatedMatchesScore(List<ResultDetail> results);
}

public class OtherInfoServiceImp : OtherInfoService
{
    public (string mainScore, string cornerScore) GetRelatedMatchesScore(List<ResultDetail> results)
    {
        var mainResult = results.Find((result) => result.ChildMatchType != SoccerChildMatchType.Corner);
        var mainScore = string.Empty;
        if (mainResult != null)
        {
            mainScore = $"main({mainResult.HomeValue}-{mainResult.AwayValue})";
        }

        var cornerResult = results.Find((result) => result.ChildMatchType == SoccerChildMatchType.Corner);
        var cornerScore = string.Empty;
        if (cornerResult != null)
        {
            cornerScore = $"corner({cornerResult.HomeValue}-{cornerResult.AwayValue})";
        }

        return (mainScore, cornerScore);
    }
}