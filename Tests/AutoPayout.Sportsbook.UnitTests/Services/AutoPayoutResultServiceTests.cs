using System.Threading.Tasks;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using Fanex.Data.Repository;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class AutoPayoutResultServiceTests
    {
        private readonly IDynamicRepository repository;
        private readonly AutoPayoutResultService autoPayoutResultService;

        public AutoPayoutResultServiceTests()
        {
            repository = Substitute.For<IDynamicRepository>();
            autoPayoutResultService = new AutoPayoutResultServiceImp(repository);
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task Insert_SettleTypeIsHalftime_ReturnsHalftimeScoreAndDoesNotThrowException(int homeScore, int awayScore)
        {
            var match = new Match
            {
                MatchId = 1,
                TypeId = 1,
                HtHomeScore = homeScore,
                HtAwayScore = awayScore,
                FinalHomeScore = 3,
                FinalAwayScore = 4
            };

            // Act
            await repository.ExecuteAsync(Arg.Do<AddAutoPayoutResultCriteria>(x => { x.HomeValue = (int)match.HtHomeScore; x.AwayValue = (int)match.HtAwayScore; }));

            // Assert
            var exception = await Record.ExceptionAsync(() => autoPayoutResultService.Insert(match, 1, 1));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(3, 4)]
        public async Task Insert_SettleTypeIsFullTime_ReturnsHalftimeScoreAndDoesNotThrowException(int homeScore, int awayScore)
        {
            var match = new Match
            {
                MatchId = 1,
                TypeId = 2,
                HtHomeScore = 1,
                HtAwayScore = 2,
                FinalHomeScore = homeScore,
                FinalAwayScore = awayScore
            };

            // Act
            await repository.ExecuteAsync(Arg.Do<AddAutoPayoutResultCriteria>(x => { x.HomeValue = (int)match.FinalHomeScore; x.AwayValue = (int)match.FinalAwayScore; }));

            // Assert
            var exception = await Record.ExceptionAsync(() => autoPayoutResultService.Insert(match, 1, 1));
            Assert.Null(exception);
        }
    }
}