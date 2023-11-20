using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class OtherInfoServiceTests
{
    [Fact]
    public void GetRelatedMatchesScore_RelatedMatches_ReturnCornerAndMainMatchInfo()
    {
        var service = new OtherInfoServiceImp();
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "00", HomeValue = "2", AwayValue = "1", ChildMatchType = 204 },
            new() { ResourceId = "00", HomeValue = "4", AwayValue = "3" }
        };

        var (actualMain, actualCorner) = service.GetRelatedMatchesScore(resultDetails);

        Assert.Equal("main(4-3)", actualMain);
        Assert.Equal("corner(2-1)", actualCorner);
    }
}