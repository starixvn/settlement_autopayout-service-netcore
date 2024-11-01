using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ValidationStatusMatchServiceTests
{
    private readonly ValidationStatusMatchService service;

    public ValidationStatusMatchServiceTests()
    {
        service = new ValidationStatusMatchServiceImp();
    }

    [Fact]
    public void VerifyStatusMatch_MatchIsFullTimeConfirmedAndNotIsHalfTimeConfirmed_ThrowsPendingException()
    {
        // Arrange
        var match = new Match
        {
            IsFullTimeConfirmed = true,
            BetStatus = "Refund HT;"
        };

        // Assert
        Assert.Throws<Match3X30Exception>(() => service.VerifyStatusMatch(match));
    }

    [Fact]
    public void VerifyStatusMatch_FullTimeMatchButHalfTimeIsNotCalculated_ThrowsHaftTimeNotDoneException()
    {
        // Arrange
        var match = new Match
        {
            SettlementStatus = 0,
            IsHalfTimeConfirmed = true,
            IsFullTimeConfirmed = true,
            HtHomeScore = 1,
            HtAwayScore = 1,
            FinalHomeScore = 1,
            FinalAwayScore = 1,
            TypeId = SettleType.FullTime,
            IsHalfTimeCalculated = false
        };

        // Assert
        Assert.Throws<HalftimeNotDoneException>(() => service.VerifyStatusMatch(match));
    }

    [Theory]
    [InlineData(2, true, 1, 1, 1)]
    [InlineData(2, true, 42, 202, 95730)]
    [InlineData(1, true, 42, 202, 95730)]
    [InlineData(1, true, 1, 1, 1)]
    public void VerifyStatusMatch_ValidMatch_DoesNotThrowException(
        int typeId, bool isHalfTimeCalculated, int leagueGroupId, int childMatchType, int leagueId)
    {
        // Arrange
        var match = new Match
        {
            SettlementStatus = 0,
            IsHalfTimeConfirmed = true,
            IsFullTimeConfirmed = true,
            HtHomeScore = 1,
            HtAwayScore = 1,
            FinalHomeScore = 1,
            FinalAwayScore = 1,
            TypeId = typeId,
            IsHalfTimeCalculated = isHalfTimeCalculated,
            LeagueGroupId = leagueGroupId,
            LeagueId = leagueId,
            ChildMatchType = childMatchType
        };

        // Assert
        var exception = Record.Exception(() => service.VerifyStatusMatch(match));
        Assert.Null(exception);
    }
}