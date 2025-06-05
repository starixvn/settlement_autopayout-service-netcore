using AutoPayout.Core.Models;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ScoreServiceTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly MatchQueryService matchQueryService;
    private readonly ScoreServiceImp service;

    public ScoreServiceTests()
    {
        var settings = new AppSettingsKey()
        {
            RetryInterval = 1000,
            RetryCount = 1,
            EnvironmentName = "Local",
        };
        var appSettingsKey1 = Options.Create(settings);
        dynamicRepository = Substitute.For<IDynamicRepository>();
        matchQueryService = Substitute.For<MatchQueryService>();
        service = new ScoreServiceImp(dynamicRepository, appSettingsKey1, matchQueryService);
    }

    [Fact]
    public async Task TryGetScore_ExistsHalftimeScore_ReturnsExpectedHalftimeScore()
    {
        // Arrange
        var match = new Match { MatchId = 1, TypeId = 1 };
        var fakeScores = new List<Score>
        {
            new() { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime, SourceId = Source.Sbo },
            new() { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime, SourceId = Source.Sbo },
            new() { MatchId = 2, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime, SourceId = Source.Sbo },
        };

        dynamicRepository.FetchAsync<Score>(Arg.Any<ICriteria>()).Returns(fakeScores);

        // Act
        var actualScore = await service.TryGetScore(match);

        // Assert
        var expectedScore = new Score { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime, SourceId = Source.Sbo };
        AssertExtension.AreEqual(expectedScore, actualScore);
    }

    [Fact]
    public async Task TryGetScore_ExistsFullTimeScore_ReturnsExpectedFullTimeScore()
    {
        // Arrange
        var match = new Match { MatchId = 1, TypeId = 2 };
        var fakeScores = new List<Score>
        {
            new() { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime,  SourceId = Source.Sbo },
            new() { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime,  SourceId = Source.Sbo },
            new() { MatchId = 2, HomeValue = "1", AwayValue = "1", TypeId = SettleType.FullTime, SourceId = Source.Sbo },
        };

        dynamicRepository.FetchAsync<Score>(Arg.Any<ICriteria>()).Returns(fakeScores);

        // Act
        var actualScore = await service.TryGetScore(match);

        // Assert
        var expectedScore = new Score { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime, SourceId = Source.Sbo };
        AssertExtension.AreEqual(expectedScore, actualScore);
    }

    [Fact]
    public async Task TryGetScore_NotExistsScore_ReturnsEmptyScore()
    {
        // Arrange
        var match = new Match { MatchId = 1, TypeId = 2 };

        // Act
        var actualScore = await service.TryGetScore(match);

        // Assert
        var expectedScore = new Score();
        AssertExtension.AreEqual(expectedScore, actualScore);
    }

    [Fact]
    public async Task TryGetRelatedResults_RelatedMatchIsCalculated_ReturnRelatedMatch()
    {
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", SettlementStatusId = 4},
            new() { ResourceId = "00", SettlementStatusId = 4}
        };
        var match = new Match { MatchId = 12345, TypeId = 1 };
        matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));

        var actual = await service.TryGetRelatedResults(match);

        Assert.Single(actual);
    }

    [Fact]
    public async Task TryGetRelatedResults_OneRelatedMatchIsNotCalculated_EmptyList()
    {
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = "01", SettlementStatusId = 2},
            new() { ResourceId = "00", SettlementStatusId = 2}
        };
        var match = new Match { MatchId = 12345, TypeId = 2 };
        matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));

        var actual = await service.TryGetRelatedResults(match);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetResultDetail_HalftimeMatch_ReturnsHalftimeResults()
    {
        // Arrange
        var match = new Match { MatchId = 1, TypeId = SettleType.Halftime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.HalfTime, SettlementStatusId = SettlementStatus.Calculated },
            new() { ResourceId = ResourceId.FullTime, SettlementStatusId = SettlementStatus.Calculated }
        };
        matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));

        // Act
        var actual = await service.GetResultDetail(match);

        // Assert
        Assert.Single(actual);
        Assert.Equal(ResourceId.HalfTime, actual.First().ResourceId);
    }

    [Fact]
    public async Task GetResultDetail_FulltimeMatch_ReturnsFulltimeResults()
    {
        // Arrange
        var match = new Match { MatchId = 1, TypeId = SettleType.FullTime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.HalfTime, SettlementStatusId = SettlementStatus.Calculated },
            new() { ResourceId = ResourceId.FullTime, SettlementStatusId = SettlementStatus.Calculated }
        };
        matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));

        // Act
        var actual = await service.GetResultDetail(match);

        // Assert
        Assert.Single(actual);
        Assert.Equal(ResourceId.FullTime, actual.First().ResourceId);
    }

    [Fact]
    public async Task GetResultDetail_GetAllTrue_ReturnsAllResults()
    {
        // Arrange
        var match = new Match { MatchId = 1, TypeId = SettleType.FullTime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.HalfTime, SettlementStatusId = SettlementStatus.Calculated },
            new() { ResourceId = ResourceId.FullTime, SettlementStatusId = SettlementStatus.Calculated }
        };
        matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));

        // Act
        var actual = await service.GetResultDetail(match, true);

        // Assert
        Assert.Equal(2, actual.Count);
    }
}