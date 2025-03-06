using AutoPayout.Result.Constants;
using AutoPayout.Result.Models;

namespace AutoPayout.Result.Services;

public interface ActionResourceService
{
    List<XmlResource> HalftimeResources(Goal goal);

    List<XmlResource> FullTimeGoalResources(Goal goal);

    ResourceItem CalculateResources(string resourceId, string homeValue, string awayValue);

    List<ResourceItem> CreateResourceByAction(int typeId);

}

public class ActionResourceServiceImp(FirstGoalService goalResultService, GoalRecordService goalRecordService)
    : ActionResourceService
{
    private const byte SettledBy = 5;

    private const byte Completed = 4;

    private const byte HalfTimeType = 1;

    public List<XmlResource> HalftimeResources(Goal goal)
    {
        var resources = new List<XmlResource>();
        var halftimeGoal = goalRecordService.HalftimeGoal(goal.GoalRecord);
        var firstGoal = goalResultService.FirstGoal(halftimeGoal);

        if (goal.IsExistHtFgLg)
        {
            var lastGoal = goalResultService.LastGoal(halftimeGoal);
            resources.Add(CreateCalculateResource(ResourceKey.HalftimeFirstGoal, firstGoal));
            resources.Add(CreateCalculateResource(ResourceKey.HalftimeLastGoal, lastGoal));
        }

        if (goal.IsExistFgNg && !string.IsNullOrWhiteSpace(halftimeGoal))
        {
            resources.Add(CreateCalculateResource(ResourceKey.FullTimeFirstGoal, firstGoal));
        }

        if (goal.IsEnabledGoalRecordHt)
        {
            resources.Add(CreateCalculateResource(ResourceKey.HalftimeGoalRecord, halftimeGoal));
        }

        return resources;
    }

    public List<XmlResource> FullTimeGoalResources(Goal goal)
    {
        var resources = new List<XmlResource>();

        if (goal.IsExistFgLg)
        {
            var fullTimeGoal = goalRecordService.FullTimeGoal(goal.GoalRecord);
            var firstGoal = goalResultService.FirstGoal(fullTimeGoal);
            var lastGoal = goalResultService.LastGoal(fullTimeGoal);
            resources.Add(CreateCalculateResource(ResourceKey.FullTimeFirstGoal, firstGoal));
            resources.Add(CreateCalculateResource(ResourceKey.FullTimeLastGoal, lastGoal));
        }

        if (goal.IsEnabledGoalRecordFt)
        {
            var resource = CreateCalculateResource(ResourceKey.FullTimeGoalRecord, goal.GoalRecord);
            resources.Add(resource);
        }

        return resources;
    }

    private static XmlResource CreateCalculateResource(string resourceId, string homeValue)
    {
        return new XmlResource
        {
            ResourceId = resourceId,
            ResultStatusId = SettledBy,
            SettlementStatusId = Completed,
            HomeValue = homeValue
        };
    }

    public ResourceItem CalculateResources(string resourceId, string homeValue, string awayValue)
    {
        return new ResourceItem
        {
            ResourceId = resourceId,
            HomeValue = homeValue,
            AwayValue = awayValue
        };
    }

    public List<ResourceItem> CreateResourceByAction(int typeId)
    {
        if (typeId == HalfTimeType)
        {
            return [new ResourceItem { ResourceId = ResourceKey.Halftime }];
        }

        return [new ResourceItem { ResourceId = ResourceKey.Halftime }, new ResourceItem { ResourceId = ResourceKey.Final }];
    }
}