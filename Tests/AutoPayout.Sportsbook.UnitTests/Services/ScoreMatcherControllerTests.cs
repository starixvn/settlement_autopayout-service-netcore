using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;
using NSubstitute.ExceptionExtensions;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ScoreMatcherControllerTests
{
    private readonly ValidationSettingMatchService _validationSettingMatchService;
    private readonly ValidationStatusMatchService _validationStatusMatchService;
    private readonly ValidationScoreService _validationScoreService;
    private readonly ScoreService _scoreService;
    private readonly ValidationGoalRecordService _validationGoalRecordService;
    private readonly MatchQueryService _matchQueryService;
    private readonly ScoreMatcherControllerImp _controller;

    public ScoreMatcherControllerTests()
    {
        _validationSettingMatchService = Substitute.For<ValidationSettingMatchService>();
        _validationStatusMatchService = Substitute.For<ValidationStatusMatchService>();
        _validationScoreService = Substitute.For<ValidationScoreService>();
        _scoreService = Substitute.For<ScoreService>();
        _validationGoalRecordService = Substitute.For<ValidationGoalRecordService>();
        _matchQueryService = Substitute.For<MatchQueryService>();

        _controller = new ScoreMatcherControllerImp(
            _validationSettingMatchService,
            _validationStatusMatchService,
            _validationScoreService,
            _scoreService,
            _validationGoalRecordService,
            _matchQueryService);
    }

    [Fact]
    public async Task Verify_ValidMatch_ReturnsSourceIdIsSbo()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        _matchQueryService.GetMatchAndResultDetail(1).Returns((new RemarkSettingModel(), new List<ResultDetail>()));
        _scoreService.TryGetScore(Arg.Any<Match>()).Returns(new Score { SourceId = Source.Sbo });

        // Act
        var actualSourceId = await _controller.Verify(match);

        // Assert
        Assert.Equal(Source.Sbo, actualSourceId);
    }

    [Fact]
    public async Task Verify_TotalCornerGoalMatch_ReturnAdminSiteId()
    {
        // Arrange
        var match = new Match { MainType = 33, ChildLeagueType = 1029, MatchId = 1 };
        _matchQueryService.GetMatchAndResultDetail(1).Returns((new RemarkSettingModel(), new List<ResultDetail>()));
        _scoreService.TryGetRelatedResults(Arg.Any<Match>()).Returns(new List<ResultDetail>());

        // Act
        var actualSourceId = await _controller.Verify(match);

        // Assert
        Assert.Equal(Source.AdminSite, actualSourceId);
    }


    [Fact]
    public async Task Verify_NonDirectSettlement_CallsScoreService()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        _scoreService.TryGetScore(match).Returns(new Score { SourceId = Source.Sbo });
        _matchQueryService.GetMatchAndResultDetail(1).Returns((new RemarkSettingModel(), new List<ResultDetail>()));

        // Act
        await _controller.Verify(match);

        // Assert
        await _scoreService.Received(1).TryGetScore(match);
        _validationScoreService.Received(1).VerifyScore(match, Arg.Any<Score>());
        _validationGoalRecordService.Received(1).VerifyGoalRecord(match);
    }

    [Fact]
    public async Task Verify_TotalCornerGoalMatch_CallsCorrectServices()
    {
        // Arrange
        var match = new Match { MainType = 33, ChildLeagueType = 1029, MatchId = 1 };
        var results = new List<ResultDetail>();
        _scoreService.TryGetRelatedResults(match).Returns(results);
        _matchQueryService.GetMatchAndResultDetail(1).Returns((new RemarkSettingModel(), new List<ResultDetail>()));

        // Act
        await _controller.Verify(match);

        // Assert
        await _scoreService.Received(1).TryGetRelatedResults(match);
        _validationScoreService.Received(1).VerifyResultsConsistent(match, results);
        await _scoreService.DidNotReceive().TryGetScore(Arg.Any<Match>());
        _validationGoalRecordService.DidNotReceive().VerifyGoalRecord(Arg.Any<Match>());
    }


    [Fact]
    public async Task Verify_NonDirectSettlement_CallsEndValidations()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        var remarkFromDb = new RemarkSettingModel();
        _scoreService.TryGetScore(match).Returns(new Score { SourceId = Source.Sbo });
        _matchQueryService.GetMatchAndResultDetail(1).Returns((remarkFromDb, new List<ResultDetail>()));

        // Act
        await _controller.Verify(match);

        // Assert
        await _matchQueryService.Received(1).GetMatchAndResultDetail(1);
        _validationSettingMatchService.Received(2).VerifySettingMatch(Arg.Any<RemarkSettingModel>());
        await _validationSettingMatchService.Received(2).VerifyHoldResult(match);
    }

    [Fact]
    public async Task Verify_WithDifferentSourceIds_ReturnsCorrectSourceId()
    {
        // Arrange
        var testCases = new[]
        {
            Source.Sbo,
            //Source.Pinnacle,
            Source.AdminSite,
            //Source.IBC
        };

        foreach (var expectedSourceId in testCases)
        {
            var match = new Match { MatchId = 1 };
            _scoreService.TryGetScore(match).Returns(new Score { SourceId = expectedSourceId });
            _matchQueryService.GetMatchAndResultDetail(1).Returns((new RemarkSettingModel(), new List<ResultDetail>()));

            // Act
            var actualSourceId = await _controller.Verify(match);

            // Assert
            Assert.Equal(expectedSourceId, actualSourceId);
        }
    }

    [Fact]
    public async Task Verify_ThrowsException_WhenValidationFails()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        _validationSettingMatchService
            .When(x => x.VerifySettingMatch(Arg.Any<RemarkSettingModel>()))
            .Do(x => throw new ArgumentException("Validation failed"));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _controller.Verify(match));
    }

    [Fact]
    public async Task Verify_ThrowsException_WhenScoreServiceFails()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        _scoreService.TryGetScore(match).Throws(new InvalidOperationException("Score service failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Verify(match));
    }

    [Fact]
    public async Task Verify_CallsServicesInCorrectOrder()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        _scoreService.TryGetScore(match).Returns(new Score { SourceId = Source.Sbo });
        _matchQueryService.GetMatchAndResultDetail(1).Returns((new RemarkSettingModel(), new List<ResultDetail>()));

        var callOrder = new List<string>();

        _validationSettingMatchService
            .When(x => x.VerifySettingMatch(Arg.Any<RemarkSettingModel>()))
            .Do(x => callOrder.Add("VerifySettingMatch"));

        _validationSettingMatchService
            .When(x => x.VerifyHoldResult(Arg.Any<Match>()))
            .Do(x => callOrder.Add("VerifyHoldResult"));

        _validationStatusMatchService
            .When(x => x.VerifyStatusMatch(Arg.Any<Match>()))
            .Do(x => callOrder.Add("VerifyStatusMatch"));

        // Act
        await _controller.Verify(match);

        // Assert
        Assert.Equal("VerifySettingMatch", callOrder[0]);
        Assert.Equal("VerifyHoldResult", callOrder[1]);
        Assert.Equal("VerifyStatusMatch", callOrder[2]);
    }

    [Fact]
    public async Task Verify_NullMatch_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => _controller.Verify(null));
    }
}