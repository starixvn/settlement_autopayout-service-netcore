using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;
using NSubstitute.ExceptionExtensions;

namespace AutoPayout.Sportsbook.UnitTests.Services;

//public class ValidationGoalRecordServiceTests
//{
//    private readonly ScoreAndGoalRecordService scoreAndGoalRecordService;
//    public ValidationGoalRecordServiceTests()
//    {
//        scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
//    }
//    [Fact]
//    public void VerifyGoalRecord_SettleHalftimeAndIsNotExistedSemiColon_ThrowsGoalRecordNotSemiColonException()
//    {
//        // Arrange
//        var match = new Match
//        {
//            GoalRecord = "AH;",
//            HtHomeScore = 2,
//            HtAwayScore = 2,
//            TypeId = SettleType.Halftime
//        };

//        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
//        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

//        // Assert
//        Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
//    }

//    [Fact]
//    public void VerifyGoalRecord_SettleHalftimeAndNotConsistentHalftime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
//    {
//        // Arrange
//        var match = new Match
//        {
//            GoalRecord = "AH;",
//            HtHomeScore = 2,
//            HtAwayScore = 2,
//            TypeId = SettleType.Halftime
//        };

//        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
//        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
//        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

//        // Assert
//        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
//    }

//    [Fact]
//    public void VerifyGoalRecord_SettleHalftimeAndIsConsistentHalftime_DoesNotThrowException()
//    {
//        // Arrange
//        var match = new Match
//        {
//            GoalRecord = "AH;",
//            HtHomeScore = 1,
//            HtAwayScore = 1,
//            TypeId = SettleType.Halftime
//        };

//        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
//        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
//        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

//        // Assert
//        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
//        Assert.Null(exception);
//    }

//    [Fact]
//    public void VerifyGoalRecord_SettleFullTimeAndNotConsistentHalftime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
//    {
//        // Arrange
//        var match = new Match
//        {
//            GoalRecord = "AH;",
//            HtHomeScore = 2,
//            HtAwayScore = 2,
//            TypeId = SettleType.FullTime
//        };

//        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
//        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

//        // Assert
//        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
//    }

//    [Fact]
//    public void VerifyGoalRecord_SettleFullTimeAndNotConsistentFullTime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
//    {
//        // Arrange
//        var match = new Match
//        {
//            GoalRecord = "AH;",
//            HtHomeScore = 2,
//            HtAwayScore = 2,
//            TypeId = SettleType.FullTime
//        };

//        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
//        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
//        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

//        // Assert
//        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
//    }

//    [Fact]
//    public void VerifyGoalRecord_SettleFullTimeAndIsConsistent_DoesNotThrowException()
//    {
//        // Arrange
//        var match = new Match
//        {
//            GoalRecord = "AH",
//            HtHomeScore = 1,
//            HtAwayScore = 1,
//            TypeId = SettleType.FullTime,

//        };

//        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
//        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(true);
//        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

//        // Assert
//        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
//        Assert.Null(exception);
//    }
//}

public class ValidationGoalRecordServiceTests
{
    private readonly ScoreAndGoalRecordService _scoreAndGoalRecordService;
    private readonly ValidationGoalRecordServiceImp _service;

    public ValidationGoalRecordServiceTests()
    {
        _scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
        _service = new ValidationGoalRecordServiceImp(_scoreAndGoalRecordService);
    }

    #region Halftime Settlement Tests

    [Fact]
    public void VerifyGoalRecord_SettleHalftimeAndIsNotExistedSemiColon_ThrowsGoalRecordNotSemiColonException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime
        };

        _scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);

        // Assert
        Assert.Throws<GoalRecordNotSemiColonException>(() => _service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_SettleHalftimeAndNotConsistentHalftime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
        _scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);

        // Assert
        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => _service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_SettleHalftimeAndIsConsistentHalftime_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.Halftime
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        _scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);

        // Assert
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyGoalRecord_HalftimeWithEmptyGoalRecord_ChecksSemiColonValidation()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "",
            TypeId = SettleType.Halftime
        };

        _scoreAndGoalRecordService.IsExistedSemiColon("").Returns(false);

        // Assert
        Assert.Throws<GoalRecordNotSemiColonException>(() => _service.VerifyGoalRecord(match));
        _scoreAndGoalRecordService.Received(1).IsExistedSemiColon("");
    }

    #endregion

    #region Full Time Settlement Tests

    [Fact]
    public void VerifyGoalRecord_SettleFullTimeAndNotConsistentHalftime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);

        // Assert
        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => _service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_SettleFullTimeAndNotConsistentFullTime_ThrowsGoalRecordFinalScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        _scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);

        // Assert
        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => _service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_SettleFullTimeAndIsConsistent_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.FullTime,
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        _scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(true);

        // Assert
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyGoalRecord_FullTimeWithComplexGoalRecord_ValidatesConsistency()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "1H;2A;3H;4A",
            HtHomeScore = 2,
            HtAwayScore = 1,
            FinalHomeScore = 3,
            FinalAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(match).Returns(true);
        _scoreAndGoalRecordService.IsConsistentFullTime(match).Returns(true);

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.Received(1).IsConsistentHalftime(match);
        _scoreAndGoalRecordService.Received(1).IsConsistentFullTime(match);
    }

    #endregion

    #region Unchecked Goal Record Tests - IsSabaPenaltyShootout

    #endregion

    #region Unchecked Goal Record Tests - IsNthCorner

    [Fact]
    public void VerifyGoalRecord_IsNthCorner_SkipsValidation()
    {
        // Arrange
        var match = new Match
        {
            TypeId = SettleType.FullTime,
            GoalRecord = "invalid_record",
            SubType = 2105
        };

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
        _scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    }

    [Fact]
    public void VerifyGoalRecord_IsNthCornerHalftime_SkipsValidation()
    {
        // Arrange
        var match = new Match
        {
            TypeId = SettleType.Halftime,
            GoalRecord = "",
            SubType = 2105

        };

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
    }

    #endregion

    #region Unchecked Goal Record Tests - ExtraTime MatchType

    [Fact]
    public void VerifyGoalRecord_ExtraTimeMatchType_SkipsValidation()
    {
        // Arrange
        var match = new Match
        {
            MatchType = MatchTypes.ExtraTime,
            TypeId = SettleType.FullTime,
            GoalRecord = "invalid_record"
        };

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
        _scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    }

    [Fact]
    public void VerifyGoalRecord_ExtraTimeMatchTypeHalftime_SkipsValidation()
    {
        // Arrange
        var match = new Match
        {
            MatchType = MatchTypes.ExtraTime,
            TypeId = SettleType.Halftime,
            GoalRecord = ""
        };

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
    }

    #endregion

    #region Unchecked Goal Record Tests - TotalGoalsMinutes MatchType

    [Fact]
    public void VerifyGoalRecord_TotalGoalsMinutesMatchType_SkipsValidation()
    {
        // Arrange
        var match = new Match
        {
            MatchType = MatchTypes.TotalGoalsMinutes,
            TypeId = SettleType.FullTime,
            GoalRecord = "invalid_record"
        };

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
        _scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    }

    [Fact]
    public void VerifyGoalRecord_TotalGoalsMinutesMatchTypeHalftime_SkipsValidation()
    {
        // Arrange
        var match = new Match
        {
            MatchType = MatchTypes.TotalGoalsMinutes,
            TypeId = SettleType.Halftime,
            GoalRecord = ""
        };

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
    }

    #endregion

    #region Combined Unchecked Conditions Tests

    //[Fact]
    //public void VerifyGoalRecord_MultipleUncheckedConditions_SkipsValidation()
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        IsSabaPenaltyShootout = true,
    //        IsNthCorner = true,
    //        MatchType = MatchTypes.ExtraTime,
    //        TypeId = SettleType.FullTime,
    //        GoalRecord = "invalid_record"
    //    };

    //    // Act
    //    var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

    //    // Assert
    //    Assert.Null(exception);
    //    _scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
    //    _scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    //}

    //[Theory]
    //[InlineData(true, false, 0)]
    //[InlineData(false, true, 0)]
    //[InlineData(false, false, MatchTypes.ExtraTime)]
    //[InlineData(false, false, MatchTypes.TotalGoalsMinutes)]
    //public void VerifyGoalRecord_VariousUncheckedConditions_SkipsValidation(
    //    bool isSabaPenaltyShootout,
    //    bool isNthCorner,
    //    byte matchType)
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        IsSabaPenaltyShootout = isSabaPenaltyShootout,
    //        IsNthCorner = isNthCorner,
    //        MatchType = matchType,
    //        TypeId = SettleType.FullTime,
    //        GoalRecord = "invalid_record"
    //    };

    //    // Act
    //    var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

    //    // Assert
    //    Assert.Null(exception);
    //    _scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
    //    _scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    //}

    #endregion

    #region Edge Cases and Error Scenarios

    [Fact]
    public void VerifyGoalRecord_UnknownTypeId_ProcessesAsFullTime()
    {
        // Arrange
        var match = new Match
        {
            TypeId = 999, // Unknown type
            GoalRecord = "1H;2A"
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(match).Returns(true);
        _scoreAndGoalRecordService.IsConsistentFullTime(match).Returns(true);

        // Act
        var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

        // Assert
        Assert.Null(exception);
        _scoreAndGoalRecordService.Received(1).IsConsistentHalftime(match);
        _scoreAndGoalRecordService.Received(1).IsConsistentFullTime(match);
    }

    //[Fact]
    //public void VerifyGoalRecord_NormalMatchFullTime_CallsBothConsistencyChecks()
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        TypeId = SettleType.FullTime,
    //        GoalRecord = "1H;2A;3H",
    //        IsSabaPenaltyShootout = false,
    //        IsNthCorner = false,
    //        MatchType = 0
    //    };

    //    _scoreAndGoalRecordService.IsConsistentHalftime(match).Returns(true);
    //    _scoreAndGoalRecordService.IsConsistentFullTime(match).Returns(true);

    //    // Act
    //    _service.VerifyGoalRecord(match);

    //    // Assert
    //    _scoreAndGoalRecordService.Received(1).IsConsistentHalftime(match);
    //    _scoreAndGoalRecordService.Received(1).IsConsistentFullTime(match);
    //    _scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
    //}

    //[Fact]
    //public void VerifyGoalRecord_NormalMatchHalftime_CallsAllHalftimeChecks()
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        TypeId = SettleType.Halftime,
    //        GoalRecord = "1H;",
    //        IsSabaPenaltyShootout = false,
    //        IsNthCorner = false,
    //        MatchType = 0
    //    };

    //    _scoreAndGoalRecordService.IsExistedSemiColon(match.GoalRecord).Returns(true);
    //    _scoreAndGoalRecordService.IsConsistentHalftime(match).Returns(true);

    //    // Act
    //    _service.VerifyGoalRecord(match);

    //    // Assert
    //    _scoreAndGoalRecordService.Received(1).IsExistedSemiColon(match.GoalRecord);
    //    _scoreAndGoalRecordService.Received(1).IsConsistentHalftime(match);
    //    _scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    //}

    [Fact]
    public void VerifyGoalRecord_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var match = new Match
        {
            TypeId = SettleType.FullTime,
            GoalRecord = "1H;2A"
        };

        _scoreAndGoalRecordService.IsConsistentHalftime(match)
            .Throws(new InvalidOperationException("Service error"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_NullMatch_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() => _service.VerifyGoalRecord(null));
    }

    #endregion

    #region Service Call Order Tests

    [Fact]
    public void VerifyGoalRecord_HalftimeExecution_CallsInCorrectOrder()
    {
        // Arrange
        var match = new Match
        {
            TypeId = SettleType.Halftime,
            GoalRecord = "1H;"
        };

        var callOrder = new List<string>();

        _scoreAndGoalRecordService
            .When(x => x.IsExistedSemiColon(Arg.Any<string>()))
            .Do(x => callOrder.Add("IsExistedSemiColon"));

        _scoreAndGoalRecordService
            .When(x => x.IsConsistentHalftime(Arg.Any<Match>()))
            .Do(x => callOrder.Add("IsConsistentHalftime"));

        _scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);

        // Act
        _service.VerifyGoalRecord(match);

        // Assert
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("IsExistedSemiColon", callOrder[0]);
        Assert.Equal("IsConsistentHalftime", callOrder[1]);
    }

    [Fact]
    public void VerifyGoalRecord_FullTimeExecution_CallsInCorrectOrder()
    {
        // Arrange
        var match = new Match
        {
            TypeId = SettleType.FullTime,
            GoalRecord = "1H;2A"
        };

        var callOrder = new List<string>();

        _scoreAndGoalRecordService
            .When(x => x.IsConsistentHalftime(Arg.Any<Match>()))
            .Do(x => callOrder.Add("IsConsistentHalftime"));

        _scoreAndGoalRecordService
            .When(x => x.IsConsistentFullTime(Arg.Any<Match>()))
            .Do(x => callOrder.Add("IsConsistentFullTime"));

        _scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        _scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(true);

        // Act
        _service.VerifyGoalRecord(match);

        // Assert
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("IsConsistentHalftime", callOrder[0]);
        Assert.Equal("IsConsistentFullTime", callOrder[1]);
    }

    #endregion

    #region Integration-style Tests

    //[Fact]
    //public void VerifyGoalRecord_RealWorldScenario_HalftimeSuccess()
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        MatchId = 12345,
    //        TypeId = SettleType.Halftime,
    //        GoalRecord = "15'H;30'A;45'H;",
    //        HtHomeScore = 2,
    //        HtAwayScore = 1,
    //        IsSabaPenaltyShootout = false,
    //        IsNthCorner = false,
    //        MatchType = 0
    //    };

    //    _scoreAndGoalRecordService.IsExistedSemiColon(match.GoalRecord).Returns(true);
    //    _scoreAndGoalRecordService.IsConsistentHalftime(match).Returns(true);

    //    // Act
    //    var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

    //    // Assert
    //    Assert.Null(exception);
    //}

    //[Fact]
    //public void VerifyGoalRecord_RealWorldScenario_FullTimeSuccess()
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        MatchId = 12345,
    //        TypeId = SettleType.FullTime,
    //        GoalRecord = "15'H;30'A;45'H;60'A;75'H",
    //        HtHomeScore = 2,
    //        HtAwayScore = 1,
    //        FinalHomeScore = 3,
    //        FinalAwayScore = 2,
    //        IsSabaPenaltyShootout = false,
    //        IsNthCorner = false,
    //        MatchType = 0
    //    };

    //    _scoreAndGoalRecordService.IsConsistentHalftime(match).Returns(true);
    //    _scoreAndGoalRecordService.IsConsistentFullTime(match).Returns(true);

    //    // Act
    //    var exception = Record.Exception(() => _service.VerifyGoalRecord(match));

    //    // Assert
    //    Assert.Null(exception);
    //}

    #endregion
}