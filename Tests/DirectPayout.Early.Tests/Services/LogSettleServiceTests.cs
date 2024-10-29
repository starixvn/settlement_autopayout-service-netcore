using AutoPayout.Sportsbook.Criteria;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using DirectPayout.Early.Services;
using Fanex.Data.Repository;
using NSubstitute;

namespace DirectPayout.Early.Tests.Services;

public class LogSettleServiceTests
{
    private readonly IDynamicRepository repository;
    private readonly LogSettleServiceImp logSettleService;

    public LogSettleServiceTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        logSettleService = new LogSettleServiceImp(repository);
    }

    [Fact]
    public async Task WriteLog_ShouldCallExecuteAsyncWithCorrectCriteria()
    {
        // Arrange
        var settleCriteria = new SettleCriteria
        {
            MatchId = 1,
            Action = "CAL",
            SelectedResources = "425, 424"
        };

        // Act
        await logSettleService.WriteLog(settleCriteria);

        // Assert
        await repository.Received(1).ExecuteAsync(Arg.Is<InsertLogCriteria>(c =>
            c.MatchId == settleCriteria.MatchId &&
            c.Action == settleCriteria.Action &&
            c.SportType == 1 &&
            c.IsAuto &&
            c.SelectedResources == settleCriteria.SelectedResources
        ));
    }

    [Fact]
    public async Task WriteLogResettle_ShouldCallExecuteAsyncWithCorrectCriteria()
    {
        // Arrange
        var match = new Match
        {
            MatchId = 1,
            ResultDetails = new List<ResultDetail>
        {
            new ResultDetail { ResourceId = "10", HomeValue = "1", AwayValue = "2" },
            new ResultDetail { ResourceId = "20", HomeValue = "3" }
        }
        };

        // Act
        await logSettleService.WriteLogResettle(match);

        // Assert
        await repository.Received(1).ExecuteAsync(Arg.Is<LogDirectlyCriteria>(c =>
            c.Doing == "NAP: Direct Payout: 10(1-2),20(3)" &&
            c.Extend == match.MatchId
        ));
    }


    [Fact]
    public async Task WriteLogErrorSettle_ShouldCallExecuteAsyncWithCorrectCriteria()
    {
        // Arrange
        var match = new Match
        {
            MatchId = 1
        };
        var error = "Error occurred during settlement";

        // Act
        await logSettleService.WriteLogErrorSettle(match, error);

        // Assert
        await repository.Received(1).ExecuteAsync(Arg.Is<LogErrorSettleCriteria>(c =>
            c.MatchId == match.MatchId &&
            c.Remark == error &&
            c.ActionId == 148
        ));
    }


}
