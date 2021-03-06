﻿using SC2APIProtocol;
using Tyr.Agents;
using Tyr.Builds.BuildLists;
using Tyr.Tasks;

namespace Tyr.Builds.Protoss
{
    public class SCVRush : Build
    {
        private SCVRushTask WorkerRushTask = new SCVRushTask();
        private int LastReinforcementsFrame = 0;
        private bool MessageSent = false;

        public override string Name()
        {
            return "SCVRush";
        }

        public override void InitializeTasks()
        {
            base.InitializeTasks();
            Tyr.Bot.TaskManager.Add(WorkerRushTask);
            WorkerRushTask.Stopped = false;
        }

        public override void OnStart(Tyr tyr)
        {
            Set += SupplyDepots();
        }

        public BuildList SupplyDepots()
        {
            BuildList result = new BuildList();
            
            result.If(() =>
            {
                return Build.FoodUsed()
                    + Tyr.Bot.UnitManager.Count(UnitTypes.COMMAND_CENTER)
                    >= Build.ExpectedAvailableFood() - 1
                    && Build.ExpectedAvailableFood() < 200;
            });
            result += new BuildingStep(UnitTypes.SUPPLY_DEPOT);
            result.Goto(0);

            return result;
        }

        public override void OnFrame(Tyr tyr)
        {
            if (!MessageSent)
                    if (tyr.Enemies().Count > 0)
                    {
                        MessageSent = true;
                        tyr.Chat("Prepare to be TICKLED! :D");
                    }

            if (tyr.Frame - LastReinforcementsFrame >= 100
                && WorkerTask.Task.Units.Count >= (tyr.EnemyStrategyAnalyzer.LiftingDetected ? 22 : 12)
                && !tyr.EnemyStrategyAnalyzer.LiftingDetected)
            {
                LastReinforcementsFrame = tyr.Frame;
                WorkerRushTask.TakeWorkers += 6;
            }
        }

        public override void Produce(Tyr tyr, Agent agent)
        {
            if (agent.Unit.UnitType == UnitTypes.COMMAND_CENTER
                && Minerals() >= 50
                && (!WorkerRushTask.Stopped || Count(UnitTypes.SCV) < 20))
                agent.Order(524);
        }
    }
}
