using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ValidationGoalRecordServiceTests
{
    private readonly ScoreAndGoalRecordService scoreAndGoalRecordService;
    public ValidationGoalRecordServiceTests()
    {
        scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
    }
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

        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Assert
        Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
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

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Assert
        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
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

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

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

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Assert
        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_SettleFullTimeAndNotConsistentFullTime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Assert
        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
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

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(true);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyGoalRecord_SabaPenaltyShootout_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime,
            AutoType = 8, // SabaSoccer.AutoDirectType
            ChildMatchType = Constants.MatchType.PenaltyShootoutHdp
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);

        // Verify that validation methods were not called
        scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
        scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
        scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    }

    [Fact]
    public void VerifyGoalRecord_SabaPenaltyShootoutOu_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.FullTime,
            AutoType = 8, // SabaSoccer.AutoDirectType
            ChildMatchType = Constants.MatchType.PenaltyShootoutOu
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);

        scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
        scoreAndGoalRecordService.DidNotReceive().IsConsistentFullTime(Arg.Any<Match>());
    }

    [Fact]
    public void VerifyGoalRecord_NthCornerWithSubType2101_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime,
            SubType = 2101 // Lower bound of first range
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);

        scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
        scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
    }

    [Fact]
    public void VerifyGoalRecord_NthCornerWithSubType2199_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.FullTime,
            SubType = 2199 // Upper bound of first range
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyGoalRecord_NthCornerWithSubType2601_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime,
            SubType = 2601 // Lower bound of second range
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

    [Fact]
    public void VerifyGoalRecord_NthCornerWithSubType2699_DoesNotThrowException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.FullTime,
            SubType = 2699 // Upper bound of second range
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);
    }

    //[Fact]
    //public void VerifyGoalRecord_ExtraTime_DoesNotThrowException()
    //{
    //    // Arrange
    //    var match = new Match
    //    {
    //        GoalRecord = "AH;",
    //        HtHomeScore = 2,
    //        HtAwayScore = 2,
    //        TypeId = SettleType.Halftime,
    //        ChildMatchType = Constants.MatchType.ExtraTime
    //    };

    //    var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

    //    // Act & Assert
    //    var exception = Record.Exception(() => service.VerifyGoalRecord(match));
    //    Assert.Null(exception);

    //    scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
    //    scoreAndGoalRecordService.DidNotReceive().IsConsistentHalftime(Arg.Any<Match>());
    //}

    [Fact]
    public void VerifyGoalRecord_HalftimeWithEmptyGoalRecord_ThrowsGoalRecordNotSemiColonException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = string.Empty,
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.Halftime
        };

        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_HalftimeWithNullGoalRecord_ThrowsGoalRecordNotSemiColonException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = null,
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.Halftime
        };

        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_HalftimeWithWhitespaceGoalRecord_ThrowsGoalRecordNotSemiColonException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "   ",
            HtHomeScore = 1,
            HtAwayScore = 1,
            TypeId = SettleType.Halftime
        };

        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_FullTimeWithEmptyGoalRecordAndConsistentHalftime_ThrowsGoalRecordFinalScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = string.Empty,
            HtHomeScore = 1,
            HtAwayScore = 1,
            FinalHomeScore = 2,
            FinalAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_FullTimeWithNullGoalRecordAndConsistentHalftime_ThrowsGoalRecordFinalScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = null,
            HtHomeScore = 1,
            HtAwayScore = 1,
            FinalHomeScore = 2,
            FinalAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_FullTimeWithNullScoresAndConsistentHalftime_ThrowsGoalRecordFinalScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = 1,
            HtAwayScore = 1,
            FinalHomeScore = null,
            FinalAwayScore = null,
            TypeId = SettleType.FullTime
        };

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_HalftimeWithNullScores_ThrowsGoalRecordHalftimeScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = null,
            HtAwayScore = null,
            TypeId = SettleType.Halftime
        };

        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_FullTimeWithNullHalftimeScores_ThrowsGoalRecordHalftimeScoreNotConsistentException()
    {
        // Arrange
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = null,
            HtAwayScore = null,
            FinalHomeScore = 2,
            FinalAwayScore = 2,
            TypeId = SettleType.FullTime
        };

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_MultipleSkipConditions_DoesNotThrowException()
    {
        // Arrange - Match with both IsSabaPenaltyShootout and IsNthCorner true
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime,
            AutoType = 8,
            ChildMatchType = Constants.MatchType.PenaltyShootoutHdp,
            SubType = 2101 // Also NthCorner
        };

        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert
        var exception = Record.Exception(() => service.VerifyGoalRecord(match));
        Assert.Null(exception);

        scoreAndGoalRecordService.DidNotReceive().IsExistedSemiColon(Arg.Any<string>());
    }

    [Fact]
    public void VerifyGoalRecord_SubTypeJustBelowNthCornerRange_DoesNotSkipValidation()
    {
        // Arrange - SubType = 2100 (just below 2101)
        var match = new Match
        {
            GoalRecord = "AH;",
            HtHomeScore = 2,
            HtAwayScore = 2,
            TypeId = SettleType.Halftime,
            SubType = 2100
        };

        scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert - Should not skip, should throw exception
        Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
    }

    [Fact]
    public void VerifyGoalRecord_SubTypeJustAboveNthCornerRange_DoesNotSkipValidation()
    {
        // Arrange - SubType = 2200 (just above 2199)
        var match = new Match
        {
            GoalRecord = "AH",
            HtHomeScore = 1,
            HtAwayScore = 1,
            FinalHomeScore = 2,
            FinalAwayScore = 2,
            TypeId = SettleType.FullTime,
            SubType = 2200
        };

        scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
        scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
        var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

        // Act & Assert - Should not skip, should throw exception
        Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
    }
}