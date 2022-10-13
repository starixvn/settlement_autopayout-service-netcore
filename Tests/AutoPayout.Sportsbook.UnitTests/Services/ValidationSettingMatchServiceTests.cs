using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ValidationSettingMatchServiceTests
{
    private readonly ValidationSettingMatchService service;

    public ValidationSettingMatchServiceTests()
    {
        service = new ValidationSettingMatchServiceImp();
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
}