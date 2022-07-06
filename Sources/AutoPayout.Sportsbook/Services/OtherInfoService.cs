using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using System.Collections.Generic;
using System.Linq;

namespace AutoPayout.Sportsbook.Services
{
    public interface OtherInfoService
    {
        (string mainScore, string cornerScore) GetRelatedMatchesScore(List<ResultDetail> results);
    }

    public class OtherInfoServiceImp : OtherInfoService
    {
        public (string mainScore, string cornerScore) GetRelatedMatchesScore(List<ResultDetail> results)
        {
            var mainResult = results.FirstOrDefault((result) => result.ChildMatchType != SoccerChildMatchType.Corner);
            var mainScore = $"main({mainResult.HomeValue}-{mainResult.AwayValue})";
            var cornerResult = results.FirstOrDefault((result) => result.ChildMatchType == SoccerChildMatchType.Corner);
            var cornerScore = $"corner({cornerResult.HomeValue}-{cornerResult.AwayValue})";

            return (mainScore, cornerScore);
        }
    }
}
