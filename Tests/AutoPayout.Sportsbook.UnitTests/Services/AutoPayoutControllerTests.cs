using AutoPayout.Sportsbook.Services;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class AutoPayoutServiceTests
    {
        private readonly MatchSettlingPoolService matchSettlingPoolService;

        private readonly MatchSettleController matchSettleController;

        private readonly AutoPayoutController controller;

        
        public AutoPayoutServiceTests()
        {
            matchSettlingPoolService = Substitute.For<MatchSettlingPoolService>();
            matchSettleController = Substitute.For<MatchSettleController>();
            controller = new AutoPayoutControllerImp(matchSettlingPoolService, matchSettleController);
        }

        [Fact]
        public async Task ProcessAsync_HasMatches_InvokeProcessMethod()
        {
            // Arrange
            var match = new Match { MatchId = 123 };
            var matches = new List<Match> { match };

            matchSettlingPoolService.PeekNewMatch().Returns(matches);

            // Act
            await controller.ProcessAsync();

            // Assert
            await matchSettleController.Received().Process(match);
        }
    }
}