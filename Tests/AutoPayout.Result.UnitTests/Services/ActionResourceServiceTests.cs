using AutoPayout.Result.Models;
using AutoPayout.Result.Services;

namespace AutoPayout.Result.UnitTests.Services;

public class ActionResourceServiceTests
{
    private readonly FirstGoalService firstGoalService;

    private readonly GoalRecordService goalRecordService;

    private static List<XmlResource> ExpectedHalftimeResourceWithExistHtFgLg()
    {
        return new List<XmlResource> {
           new XmlResource
            {
                ResourceId = "127F",
                ResultStatusId = 5,
                SettlementStatusId = 4,
                HomeValue = "A"
            },

          new XmlResource
            {
                ResourceId = "127L",
                ResultStatusId = 5,
                SettlementStatusId = 4,
                HomeValue = "H"
            }
        };
    }

    private static List<XmlResource> ExpectedHalftimeResourceWithExistFgNg()
    {
        return new List<XmlResource> {
            new XmlResource
            {
                ResourceId = "14F",
                ResultStatusId = 5,
                SettlementStatusId = 4,
                HomeValue = "A"
            }
        };
    }

    private static List<XmlResource> ExpectedFullTimeResource()
    {
        return new List<XmlResource> {
        new XmlResource
        {
            ResourceId = "14F",
            ResultStatusId = 5,
            SettlementStatusId = 4,
            HomeValue = "A"
        },

          new XmlResource
        {
            ResourceId = "14L",
            ResultStatusId = 5,
            SettlementStatusId = 4,
            HomeValue = "A"
        }
        };
    }

    private static List<XmlResource> FullTimeGoalRecordResources()
    {
        return new List<XmlResource>{
                new XmlResource
                {
                    ResourceId = "08",
                    ResultStatusId = 5,
                    SettlementStatusId = 4,
                    HomeValue = "AHH;HA"
        }};
    }

    private List<XmlResource> HalftimeGoalRecordResources()
    {
        return new List<XmlResource>{
            new XmlResource{
            ResourceId = "09",
            ResultStatusId = 5,
            SettlementStatusId = 4,
            HomeValue = "AHH"
        }};
    }

    public ActionResourceServiceTests()
    {
        firstGoalService = Substitute.For<FirstGoalService>();
        goalRecordService = Substitute.For<GoalRecordService>();
    }

    [Fact]
    public void HalftimeResources_ExistHtFgLg_ReturnsExpectedResources()
    {
        // Arrange
        var goal = new Goal { IsExistHtFgLg = true };
        firstGoalService.FirstGoal(Arg.Any<string>()).Returns("A");
        firstGoalService.LastGoal(Arg.Any<string>()).Returns("H");
        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.HalftimeResources(goal);

        // Assert
        var expectedResources = ExpectedHalftimeResourceWithExistHtFgLg();
        AssertExtension.AreEqual(expectedResources, actualResources);
    }

    [Fact]
    public void HalftimeResources_IsExistFgNg_ReturnsExpectedResources()
    {
        // Arrange
        var goal = new Goal { IsExistFgNg = true };
        firstGoalService.FirstGoal(Arg.Any<string>()).Returns("A");
        goalRecordService.HalftimeGoal(Arg.Any<string>()).Returns("AHH");

        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.HalftimeResources(goal);

        // Assert
        var expectedResources = ExpectedHalftimeResourceWithExistFgNg();
        AssertExtension.AreEqual(expectedResources, actualResources);
    }

    [Fact]
    public void HalftimeResources_IsEnabledGoalRecordHt_ReturnsExpectedResources()
    {
        // Arrange
        var goal = new Goal { IsEnabledGoalRecordHt = true };
        goalRecordService.HalftimeGoal(Arg.Any<string>()).Returns("AHH");
        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.HalftimeResources(goal);

        // Assert
        AssertExtension.AreEqual(HalftimeGoalRecordResources(), actualResources);
    }

    [Fact]
    public void HalftimeResources_IsNotExistHtFgLgAndNotEnabledGoalRecordHt_ReturnsEmpty()
    {
        // Arrange
        var goal = new Goal { IsEnabledGoalRecordHt = false, IsExistHtFgLg = false };
        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.HalftimeResources(goal);

        // Assert
        Assert.Empty(actualResources);
    }

    [Fact]
    public void HalftimeResources_IsExistHtFgLgAndEnabledGoalRecordHt_ReturnsThreeResourceItem()
    {
        // Arrange
        var goal = new Goal { IsEnabledGoalRecordHt = true, IsExistHtFgLg = true };
        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.HalftimeResources(goal);

        // Assert
        Assert.True(actualResources.Count == 3);
    }

    [Fact]
    public void FullTimeGoalResources_ExistFgLg_ReturnsExpectedResources()
    {
        // Arrange
        var goal = new Goal { IsExistFgLg = true, GoalRecord = "AHH;HA" };
        firstGoalService.FirstGoal(Arg.Any<string>()).Returns("A");
        firstGoalService.LastGoal(Arg.Any<string>()).Returns("A");

        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.FullTimeGoalResources(goal);

        // Assert
        var expectedResources = ExpectedFullTimeResource();

        AssertExtension.AreEqual(expectedResources, actualResources);
    }

    [Fact]
    public void FullTimeGoalResources_IsEnabledGoalRecordFt_ReturnsExpectedResources()
    {
        // Arrange
        var goal = new Goal { IsEnabledGoalRecordFt = true, GoalRecord = "AHH;HA" };

        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.FullTimeGoalResources(goal);

        // Assert
        AssertExtension.AreEqual(FullTimeGoalRecordResources(), actualResources);
    }

    [Fact]
    public void FullTimeGoalResources_IsNotExistFgLgAndNotEnabledGoalRecordFt_ReturnsEmptyResources()
    {
        // Arrange
        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.FullTimeGoalResources(new Goal());

        // Assert
        Assert.Empty(actualResources);
    }

    [Fact]
    public void FullTimeGoalResources_IsExistFgLgAndEnabledGoalRecordFt_ReturnsThreeResourceItem()
    {
        // Arrange
        var goal = new Goal { IsExistFgLg = true, IsEnabledGoalRecordFt = true };
        var service = new ActionResourceServiceImp(firstGoalService, goalRecordService);

        // Act
        var actualResources = service.FullTimeGoalResources(goal);

        // Assert
        Assert.True(actualResources.Count == 3);
    }
}