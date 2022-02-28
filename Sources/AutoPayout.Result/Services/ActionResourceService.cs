using AutoPayout.Result.Constants;
using AutoPayout.Result.Models;
using System.Collections.Generic;

namespace AutoPayout.Result.Services
{
    public interface ActionResourceService
    {
        List<XmlResource> HalftimeResources(Goal goal);

        List<XmlResource> FullTimeGoalResources(Goal goal);
    }

    public class ActionResourceServiceImp : ActionResourceService
    {
        private const byte SettledBy = 5;

        private const byte Completed = 4;

        private readonly FirstGoalService goalResultService;

        private readonly GoalRecordService goalRecordService;

        public ActionResourceServiceImp(FirstGoalService goalResultService, GoalRecordService goalRecordService)
        {
            this.goalResultService = goalResultService;
            this.goalRecordService = goalRecordService;
        }

        public List<XmlResource> HalftimeResources(Goal goal)
        {
            var resources = new List<XmlResource>();

            var halftimeGoal = goalRecordService.HalftimeGoal(goal.GoalRecord);

            if (goal.IsExistHtFgLg)
            {
                var fristGoal = goalResultService.FirstGoal(halftimeGoal);
                var lastGoal = goalResultService.LastGoal(halftimeGoal);
                resources.Add(CreateCalculateResource(ResourceKey.HalftimeFirstGoal, fristGoal));
                resources.Add(CreateCalculateResource(ResourceKey.HalftimeLastGoal, lastGoal));
            }

            if (goal.IsEnabledGoalRecordHt)
            {
                var resource = CreateCalculateResource(ResourceKey.HalftimeGoalRecord, halftimeGoal);
                resources.Add(resource);
            }

            return resources;
        }

        public List<XmlResource> FullTimeGoalResources(Goal goal)
        {
            var resources = new List<XmlResource>();

            if (goal.IsExistFgLg)
            {
                var fullTimeGoal = goalRecordService.FullTimeGoal(goal.GoalRecord);
                var fristGoal = goalResultService.FirstGoal(fullTimeGoal);
                var lastGoal = goalResultService.LastGoal(fullTimeGoal);
                resources.Add(CreateCalculateResource(ResourceKey.FullTimeFirstGoal, fristGoal));
                resources.Add(CreateCalculateResource(ResourceKey.FullTimeLastGoal, lastGoal));
            }

            if (goal.IsEnabledGoalRecordFt)
            {
                var resource = CreateCalculateResource(ResourceKey.FullTimeGoalRecord, goal.GoalRecord);
                resources.Add(resource);
            }

            return resources;
        }

        private XmlResource CreateCalculateResource(string resourceId, string homeValue)
        {
            return new XmlResource
            {
                ResourceId = resourceId,
                ResultStatusId = SettledBy,
                SettlementStatusId = Completed,
                HomeValue = homeValue
            };
        }
    }
}