﻿using Tyr.Agents;
using static Tyr.Builds.BuildLists.ConditionalStep;

namespace Tyr.Builds.BuildLists
{
    public class ProtossBuildUtil
    {
        public static BuildList Pylons(Test condition = null)
        {
            BuildList result = new BuildList();

            result.If(() =>
            {
                return Build.FoodUsed()
                    + Tyr.Bot.UnitManager.Count(UnitTypes.NEXUS)
                    + Tyr.Bot.UnitManager.Count(UnitTypes.GATEWAY) * 2
                    + Tyr.Bot.UnitManager.Count(UnitTypes.STARGATE) * 2
                    + Tyr.Bot.UnitManager.Count(UnitTypes.ROBOTICS_FACILITY) * 2
                    >= Build.ExpectedAvailableFood() - 2
                    && Build.ExpectedAvailableFood() < 200;
            });
            if (condition != null)
                result.If(condition);
            result += new BuildingStep(UnitTypes.PYLON);
            result.Goto(0);

            return result;
        }

        public static BuildList Nexus(int total)
        {
            BuildList result = new BuildList();

            result += new BuildingStep(UnitTypes.NEXUS, total);

            return result;
        }

        public static BuildList Nexus(int total, ConditionalStep.Test test)
        {
            BuildList result = new BuildList();

            result += new ConditionalStep(test);
            result += new BuildingStep(UnitTypes.NEXUS, total);

            return result;
        }
    }
}
