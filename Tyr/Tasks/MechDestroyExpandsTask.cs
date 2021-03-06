﻿using SC2APIProtocol;
using System.Collections.Generic;
using Tyr.Agents;
using Tyr.Managers;
using Tyr.MapAnalysis;
using Tyr.Util;

namespace Tyr.Tasks
{
    class MechDestroyExpandsTask : Task
    {
        public static MechDestroyExpandsTask Task = new MechDestroyExpandsTask();
        private List<Point2D> Bases = new List<Point2D>();

        public int MaxSize = 12;
        public int RequiredSize = 12;
        public int RetreatSize = 4;
        public uint UnitType = UnitTypes.HELLBAT;
        
        public static void Enable()
        {
            Enable(Task);
        }

        public MechDestroyExpandsTask() : base(10)
        { }

        public override bool DoWant(Agent agent)
        {
            return agent.Unit.UnitType == UnitType && Units.Count < MaxSize;
        }

        public override bool IsNeeded()
        {
            Tyr.Bot.DrawText("Mines needed: " + (Tyr.Bot.Build.Completed(UnitType) >= RequiredSize));
            return Tyr.Bot.Build.Completed(UnitType) >= RequiredSize;
        }

        public override List<UnitDescriptor> GetDescriptors()
        {
            List<UnitDescriptor> result = new List<UnitDescriptor>();
            result.Add(new UnitDescriptor() { Count = MaxSize - Units.Count, UnitTypes = new HashSet<uint>() { UnitType } });
            return result;
        }

        public override void OnFrame(Tyr tyr)
        {
            Tyr.Bot.DrawText("Mines attacking: " + Units.Count);
            for (int i = Bases.Count - 1; i >= 0; i--)
            {
                if (tyr.TargetManager.PotentialEnemyStartLocations.Count == 1
                    && SC2Util.DistanceSq(Bases[i], tyr.TargetManager.PotentialEnemyStartLocations[0]) <= 25 * 25)
                {
                    Bases.RemoveAt(i);
                    continue;
                }

                bool closeEnemy = false;
                foreach (Unit enemy in tyr.Enemies())
                {
                    if (!UnitTypes.BuildingTypes.Contains(enemy.UnitType))
                        continue;
                    if (SC2Util.DistanceSq(enemy.Pos, Bases[i]) <= 8 * 8)
                    {
                        closeEnemy = true;
                        break;
                    }
                }
                if (closeEnemy)
                    continue;

                bool closeAlly = false;
                foreach (Agent agent in tyr.UnitManager.Agents.Values)
                    if (agent.DistanceSq(Bases[i]) <= 4 * 4)
                    {
                        closeAlly = true;
                        break;
                    }
                if (closeAlly)
                    Bases.RemoveAt(i);
            }

            if (Bases.Count == 0)
                foreach (BaseLocation b in tyr.MapAnalyzer.BaseLocations)
                    Bases.Add(b.Pos);
            
            if (units.Count <= RetreatSize)
            {
                Clear();
                return;
            }

            float distance = 1000000;
            Point2D target = null;
            foreach (BuildingLocation building in tyr.EnemyManager.EnemyBuildings.Values)
            {
                if (tyr.TargetManager.PotentialEnemyStartLocations.Count == 1
                       && SC2Util.DistanceSq(building.Pos, tyr.TargetManager.PotentialEnemyStartLocations[0]) <= 30 * 30)
                    continue;

                foreach (Agent agent in units)
                {
                    float dist = agent.DistanceSq(building.Pos);
                    if (dist < distance)
                    {
                        distance = dist;
                        target = SC2Util.To2D(building.Pos);
                    }
                }
            }
            
            foreach (Point2D b in Bases)
            {
                foreach (Agent agent in units)
                {
                    float dist = agent.DistanceSq(b);
                    if (dist < distance)
                    {
                        distance = dist;
                        target = b;
                    }
                }
            }

            foreach (Agent agent in units)
                tyr.MicroController.Attack(agent, target);
        }
    }
}
