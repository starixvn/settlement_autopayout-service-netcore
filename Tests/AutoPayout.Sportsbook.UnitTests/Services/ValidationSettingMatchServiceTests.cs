using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ValidationSettingMatchServiceTests
{
    private readonly ValidationSettingMatchService service;

    private readonly ScoreService scoreService;
    public ValidationSettingMatchServiceTests()
    {
        scoreService = Substitute.For<ScoreService>();
        service = new ValidationSettingMatchServiceImp(scoreService);
    }

    [Theory]
    [InlineData("00001")]
    [InlineData("0000001")]
    [InlineData("10000")]
    [InlineData("00100")]
    [InlineData("01000")]
    public void VerifySettingMatch_MatchIsHtdHold_ThrowsException(string settlementStatus)
    {
        // Arrange
        var remark = new RemarkSettingModel
        {
            SettlementStatus = settlementStatus,
        };

        // Assert
        Assert.Throws<RemarkException>(() => service.VerifySettingMatch(remark));
    }

    [Fact]
    public void VerifySettingMatch_SettingAllOff_DoesNotThrowsException()
    {
        // Arrange
        var remark = new RemarkSettingModel
        {
            SettlementStatus = "00000",
        };

        // Assert
        var exception = Record.Exception(() => service.VerifySettingMatch(remark));
        Assert.Null(exception);
    }
    [Fact]
    public async Task VerifyHoldResult_HalftimeMatchWithHold_ThrowsException()
    {
        // Arrange
        var match = new Match { TypeId = SettleType.Halftime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.HalfTime,  ResultSetting = 1}
        };
        scoreService.GetResultDetail(match, true).Returns(resultDetails);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HoldResultException>(() => service.VerifyHoldResult(match));
        Assert.Contains("HT", exception.Message);
    }

    [Fact]
    public async Task VerifyHoldResult_FulltimeMatchWithHold_ThrowsException()
    {
        // Arrange
        var match = new Match { TypeId = SettleType.FullTime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.FullTime,  ResultSetting = 1 }
        };
        scoreService.GetResultDetail(match, true).Returns(resultDetails);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HoldResultException>(() => service.VerifyHoldResult(match));
        Assert.Contains("FT", exception.Message);
    }

    [Fact]
    public async Task VerifyHoldResult_FulltimeMatchWithHoldOnHalfTime_ThrowsException()
    {
        // Arrange
        var match = new Match { TypeId = SettleType.FullTime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.HalfTime, ResultSetting = 1 }
        };
        scoreService.GetResultDetail(match, true).Returns(resultDetails);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HoldResultException>(() => service.VerifyHoldResult(match));
        Assert.Contains("HT", exception.Message);
    }



    [Fact]
    public async Task VerifyHoldResult_NoHold_DoesNotThrowException()
    {
        // Arrange
        var match = new Match { TypeId = SettleType.FullTime };
        var resultDetails = new List<ResultDetail>
        {
            new() { ResourceId = ResourceId.HalfTime,  ResultSetting = 0 },
            new() { ResourceId = ResourceId.FullTime, ResultSetting = 0 }
        };
        scoreService.GetResultDetail(match, true).Returns(resultDetails);

        // Act
        var exception = await Record.ExceptionAsync(() => service.VerifyHoldResult(match));

        // Assert
        Assert.Null(exception);
    }
}