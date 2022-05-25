using Fanex.Data.Repository;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class ScoreServiceTests
    {
        private readonly IOptions<AppSettingsKey> appSettingsKey;
        public ScoreServiceTests()
        {
            var settings = new AppSettingsKey()
            {
                RetryInterval = 1000,
                RetryCount = 1,
                EnvironmentName = "Local"
            };
            appSettingsKey = Options.Create(settings);
        }

        [Fact]
        public async Task TryGetScore_ExistsHalftimeScore_ReturnsExpectedHalftimeScore()
        {
            // Arrange
            var match = new Match { MatchId = 1, TypeId = 1 };
            var fakeScores = new List<Score>
            {
                new Score { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime, SourceId = Source.Sbo },
                new Score { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime, SourceId = Source.Sbo },
                new Score { MatchId = 2, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime, SourceId = Source.Sbo },
            };

            var dynamicRepository = Substitute.For<IDynamicRepository>();
            dynamicRepository.FetchAsync<Score>(Arg.Any<ICriteria>()).Returns(fakeScores);
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey);

            // Act
            var actualScore = await service.TryGetScore(match);

            // Assert
            var expectedScore = new Score { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime, SourceId = Source.Sbo };
            AssertExtension.AreEqual(expectedScore, actualScore);
        }

        [Fact]
        public async Task TryGetScore_ExistsFullTimeScore_ReturnsExpectedFullTimeScore()
        {
            // Arrange
            var match = new Match { MatchId = 1, TypeId = 2 };
            var emptyScores = new List<Score>();
            var fakeScores = new List<Score>
            {
                new Score { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime,  SourceId = Source.Sbo },
                new Score { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime,  SourceId = Source.Sbo },
                new Score { MatchId = 2, HomeValue = "1", AwayValue = "1", TypeId = SettleType.FullTime, SourceId = Source.Sbo },
            };

            var dynamicRepository = Substitute.For<IDynamicRepository>();
            dynamicRepository.FetchAsync<Score>(Arg.Any<ICriteria>()).Returns(fakeScores);
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey);

            // Act
            var actualScore = await service.TryGetScore(match);

            // Assert
            var expectedScore = new Score { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime, SourceId = Source.Sbo };
            AssertExtension.AreEqual(expectedScore, actualScore);
        }

        [Fact]
        public async Task TryGetScore_NotExistsScore_ReturnsEmptyScore()
        {
            // Arrange
            var match = new Match { MatchId = 1, TypeId = 2 };
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            dynamicRepository.FetchAsync<Score>(Arg.Is<ICriteria>(x => x != null)).Returns(new List<Score>());
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey);

            // Act
            var actualScore = await service.TryGetScore(match);

            // Assert
            var expectedScore = new Score();
            AssertExtension.AreEqual(expectedScore, actualScore);
        }
    }
}