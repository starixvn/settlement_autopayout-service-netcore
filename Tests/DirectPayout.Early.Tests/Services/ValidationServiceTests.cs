using AutoPayout.Core.Models;
using DirectPayout.Early.Constants;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using DirectPayout.Early.Services;
using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut.Models;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DirectPayout.Early.Tests.Services;

public class ValidationServiceTests
{
    private readonly IDynamicRepository repository;
    private readonly ScaleOutSettleService scaleOutSettleService;
    private readonly ValidationServiceImp validationService;
    private readonly AppSettingsKey appSettings;

    public ValidationServiceTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        scaleOutSettleService = Substitute.For<ScaleOutSettleService>();
        var xmlConvertService = Substitute.For<XmlConvertService>();
        var autoPayoutResultService = Substitute.For<AutoPayoutResultService>();

        appSettings = new AppSettingsKey
        {
            RetryCount = 3,
            RetryInterval = 1
        };

        var options = Options.Create(appSettings);
        validationService = new ValidationServiceImp(
            repository,
            xmlConvertService,
            scaleOutSettleService,
            options,
            autoPayoutResultService
        );
    }

    [Fact]
    public async Task VerifyEventAndTicket_ShouldSucceedWithoutRetry()
    {
        // Arrange
        var criteria = new SettleCriteria { MatchId = 1, Action = "CAL", };
        var expectedResponse = new ResultMessage { ErrorCode = 0 };
        scaleOutSettleService.ValidateEventAndTicket(Arg.Any<EventTicketRequest>())
                             .Returns(expectedResponse);

        // Act
        await validationService.VerifyEventAndTicket(criteria);

        // Assert
        await scaleOutSettleService.Received(1).ValidateEventAndTicket(Arg.Any<EventTicketRequest>());
    }

    [Fact]
    public async Task VerifyEventAndTicket_ShouldRetryOnMatchInQueueError()
    {
        // Arrange
        var criteria = new SettleCriteria { MatchId = 1, Action = "CAL" };
        var errorResponse = new ResultMessage { ErrorCode = ErrorCodes.MatchInQueue };
        var successResponse = new ResultMessage { ErrorCode = 0 };

        scaleOutSettleService.ValidateEventAndTicket(Arg.Any<EventTicketRequest>())
                             .Returns(errorResponse, errorResponse, successResponse);

        // Act
        await validationService.VerifyEventAndTicket(criteria);

        // Assert
        await scaleOutSettleService.Received(appSettings.RetryCount).ValidateEventAndTicket(Arg.Any<EventTicketRequest>());
    }

    [Fact]
    public async Task VerifyEventAndTicket_ShouldThrowAppExceptionOnErrorCode()
    {
        // Arrange
        var criteria = new SettleCriteria { MatchId = 1, Action = "CAL" };
        var errorResponse = new ResultMessage { ErrorCode = 1, ErrorMessage = "Test Error" };

        scaleOutSettleService.ValidateEventAndTicket(Arg.Any<EventTicketRequest>())
                             .Returns(errorResponse);

        // Act & Assert
        await Assert.ThrowsAsync<AppException>(() => validationService.VerifyEventAndTicket(criteria));
    }

    [Fact]
    public async Task VerifyNoRunningOdds_ShouldSucceedWithoutError()
    {
        // Arrange
        var match = new Match
        {
            MatchId = 1,
            ResultDetails = new List<ResultDetail> { new ResultDetail { ResourceId = "10" } }
        };

        repository.When(r => r.ExecuteAsync(Arg.Any<VerifyNoRunningOddsCriteria>()))
                  .Do(info => ((VerifyNoRunningOddsCriteria)info[0]).ErrCode = 0);

        // Act
        await validationService.VerifyNoRunningOdds(match);

        // Assert
        await repository.Received(1).ExecuteAsync(Arg.Any<VerifyNoRunningOddsCriteria>());
    }

    [Fact]
    public async Task VerifyNoRunningOdds_ShouldThrowRunningOddsExceptionOnErrorCode()
    {
        // Arrange
        var match = new Match
        {
            MatchId = 1,
            ResultDetails = new List<ResultDetail> { new ResultDetail { ResourceId = "10" } }
        };

        repository.When(r => r.ExecuteAsync(Arg.Any<VerifyNoRunningOddsCriteria>()))
                  .Do(info => ((VerifyNoRunningOddsCriteria)info[0]).ErrCode = 1);

        // Act & Assert
        await Assert.ThrowsAsync<RunningOddsException>(() => validationService.VerifyNoRunningOdds(match));
    }
}