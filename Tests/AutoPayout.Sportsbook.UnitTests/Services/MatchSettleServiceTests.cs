using AutoMapper;
using AutoPayout.Result.Models;
using AutoPayout.Result.Services;
using Kaden.XmlConverter;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class MatchSettleServiceTests
    {

        private readonly MatchSettleCriteriaService matchSettleCriteriaService;

        private readonly ActionResourceService actionResourceService;

        private readonly XmlConvertService xmlConvertService;

        private MatchSettleService service;

        private readonly IMapper mapper;

        public MatchSettleServiceTests()
        {
            var mapperConfig = new MapperConfigurationExpression();
            mapperConfig.AddProfile<AutoMapperProfile>();
            var config = new MapperConfiguration(mapperConfig);
            mapper = new Mapper(config);

            xmlConvertService = new XmlConvertServiceImp();
            actionResourceService = Substitute.For<ActionResourceService>();
            actionResourceService.HalftimeResources(Arg.Any<Goal>()).Returns(new List<XmlResource>());
            actionResourceService.FullTimeGoalResources(Arg.Any<Goal>()).Returns(new List<XmlResource>());
            matchSettleCriteriaService = Substitute.For<MatchSettleCriteriaService>();
        }

        private readonly List<XmlResource> FakeResources = new List<XmlResource>
        {
              new XmlResource {
                  ResourceId ="127F", HomeValue = "A", SettlementStatusId=4, ResultStatusId =5
              },
              new XmlResource {
                  ResourceId ="127L", HomeValue = "A", SettlementStatusId=4, ResultStatusId =5
              }
        };


        [Fact]
        public async Task Settle_IsFail_ReturnsResultWithErrCode()
        {
            // Arrange
            var match = new Match();
            var matchQueryService = Substitute.For<MatchQueryService>();
            matchQueryService.GetMatchInfo(Arg.Any<int>()).Returns(new MatchChecking());
            matchQueryService.Execute(Arg.Any<MatchSettleCriteria>()).Returns(new Models.Result { ErrCode = 1 });

            var criteriaResult = new MatchSettleCriteria(new Match());
            matchSettleCriteriaService.CreateSettleCriteria(Arg.Any<Match>(), Arg.Any<MatchChecking>()).Returns(criteriaResult);
            
            service = new MatchSettleServiceImp(
            matchQueryService,
            matchSettleCriteriaService,
            actionResourceService,
            xmlConvertService, 
            mapper);

            // Act
            var result = await service.Settle(match, 1);

            // Assert
            await matchQueryService.DidNotReceive().GetHedgeMatchId(1);
            Assert.True(result.ErrCode > 0);
        }

        [Fact]
        public async Task Settle_IsSuccessAndNotHedgeMatch_CallOnceExecuteMethodAndRightResourceResult()
        {
            // Arrange
            var match = new Match { TypeId = 1 };
            var hedgeMatch = new MatchHedgeDetail();
            var matchQueryService = Substitute.For<MatchQueryService>();
            matchQueryService.GetMatchInfo(Arg.Any<int>()).Returns(new MatchChecking());
            matchQueryService.GetHedgeMatchId(Arg.Any<int>()).Returns(hedgeMatch);
            matchQueryService.Execute(Arg.Any<MatchSettleCriteria>()).Returns(new Models.Result());

            var criteriaResult = new MatchSettleCriteria(match);
            matchSettleCriteriaService.CreateSettleCriteria(Arg.Any<Match>(), Arg.Any<MatchChecking>()).Returns(criteriaResult);
            actionResourceService.HalftimeResources(Arg.Any<Goal>()).Returns(FakeResources);
            actionResourceService.FullTimeGoalResources(Arg.Any<Goal>()).Returns(FakeResources);
           
            service = new MatchSettleServiceImp(
            matchQueryService,
            matchSettleCriteriaService,
            actionResourceService,
            xmlConvertService,
            mapper);

            // Act
            var result = await service.Settle(match, 1);

            // Assert
            await matchQueryService.Received(1).Execute(Arg.Any<MatchSettleCriteria>());
            Assert.True(result.ErrCode == 0);
        }

        [Fact]
        public async Task Settle_IsSuccessAndHedgeMatch_ReturnsErrCodeIsZeroAndInvokeTwiceExecuteMethod()
        {
            // Arrange
            var match = new Match { TypeId = 1 };
            var hedgeMatch = new MatchHedgeDetail { MatchId = 2 };
            var matchQueryService = Substitute.For<MatchQueryService>();
            matchQueryService.GetMatchInfo(Arg.Any<int>()).Returns(new MatchChecking());
            matchQueryService.GetHedgeMatchId(Arg.Any<int>()).Returns(hedgeMatch);
            matchQueryService.Execute(Arg.Any<MatchSettleCriteria>()).Returns(new Models.Result());

            var criteriaResult = new MatchSettleCriteria(match);
            matchSettleCriteriaService.CreateSettleCriteria(Arg.Any<Models.Match>(), Arg.Any<MatchChecking>()).Returns(criteriaResult);
            
            service = new MatchSettleServiceImp(
            matchQueryService,
            matchSettleCriteriaService,
            actionResourceService,
            xmlConvertService,
            mapper);

            // Act
            var result = await service.Settle(match, 1);

            // Assert
            await matchQueryService.Received(2).Execute(Arg.Any<MatchSettleCriteria>());
            Assert.True(result.ErrCode == 0);
        }
    }
}