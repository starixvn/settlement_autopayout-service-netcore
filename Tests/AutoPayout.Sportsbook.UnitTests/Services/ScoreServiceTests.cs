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
                EnvironmentName = "Local",
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
            var matchQueryService = Substitute.For<MatchQueryService>();
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey, matchQueryService);

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
            var fakeScores = new List<Score>
            {
                new Score { MatchId = 1, HomeValue = "1", AwayValue = "0", TypeId = SettleType.FullTime,  SourceId = Source.Sbo },
                new Score { MatchId = 1, HomeValue = "1", AwayValue = "1", TypeId = SettleType.Halftime,  SourceId = Source.Sbo },
                new Score { MatchId = 2, HomeValue = "1", AwayValue = "1", TypeId = SettleType.FullTime, SourceId = Source.Sbo },
            };

            var dynamicRepository = Substitute.For<IDynamicRepository>();
            dynamicRepository.FetchAsync<Score>(Arg.Any<ICriteria>()).Returns(fakeScores);
            var matchQueryService = Substitute.For<MatchQueryService>();
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey, matchQueryService);

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
            dynamicRepository.FetchAsync<Score>(Arg.Any<ICriteria>()).Returns(new List<Score>());
            var matchQueryService = Substitute.For<MatchQueryService>();
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey, matchQueryService);

            // Act
            var actualScore = await service.TryGetScore(match);

            // Assert
            var expectedScore = new Score();
            AssertExtension.AreEqual(expectedScore, actualScore);
        }

        [Fact]
        public async Task TryGetRelatedResults_RelatedMatchIsCalculated_ReturnRelatedMatch()
        {
            var resultDetails = new List<ResultDetail>
            {
                new ResultDetail { ResourceId = "01", SettlementStatusId = 4},
                new ResultDetail { ResourceId = "00", SettlementStatusId = 4}
            };
            var match = new Match { MatchId = 12345, TypeId = 1 };
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            var matchQueryService = Substitute.For<MatchQueryService>();
            matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey, matchQueryService);

            var actual = await service.TryGetRelatedResults(match);

            Assert.Single(actual);
        }

        [Fact]
        public async Task TryGetRelatedResults_OneRelatedMatchIsNotCalculated_EmptyList()
        {
            var resultDetails = new List<ResultDetail>
            {
                new ResultDetail { ResourceId = "01", SettlementStatusId = 2},
                new ResultDetail { ResourceId = "00", SettlementStatusId = 2}
            };
            var match = new Match { MatchId = 12345, TypeId = 2 };
            var dynamicRepository = Substitute.For<IDynamicRepository>();
            var matchQueryService = Substitute.For<MatchQueryService>();
            matchQueryService.GetMatchAndResultDetail(Arg.Any<int>()).Returns((new RemarkSettingModel(), resultDetails));
            var service = new ScoreServiceImp(dynamicRepository, appSettingsKey, matchQueryService);

            var actual = await service.TryGetRelatedResults(match);

            Assert.Empty(actual);
        }
    }
}