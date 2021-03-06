﻿using SC2APIProtocol;
using System.Collections.Generic;
using Tyr.Agents;
using Tyr.Util;

namespace Tyr.Micro
{
    public class SwarmHostController : CustomController
    {
        private int LocustUpdateFrame = 0;
        private int LastLocustFrame = 0;

        HashSet<ulong> SeenLocusts = new HashSet<ulong>();

        public override bool DetermineAction(Agent agent, Point2D target)
        {
            if (agent.Unit.UnitType != UnitTypes.SWARM_HOST)
                return false;

            UpdateLocusts();

            float distance = 15 * 15;
            Unit closeEnemy = null;
            foreach (Unit unit in Tyr.Bot.Enemies())
            {
                if (!UnitTypes.CanAttackGround(unit.UnitType))
                    continue;

                float newDist = SC2Util.DistanceSq(unit.Pos, agent.Unit.Pos);
                if (newDist > distance)
                    continue;

                closeEnemy = unit;
                distance = newDist;
            }

            if (closeEnemy != null && distance <= 12 * 12)
            {
                agent.Order(Abilities.MOVE, agent.From(closeEnemy, 4));
                return true;
            }

            if (Tyr.Bot.Frame - LastLocustFrame < 22.4 || Tyr.Bot.Frame - LastLocustFrame > 40 * 22.4 + 11)
            {
                float targetDistance = 15 * 15;
                Unit targetEnemy = null;
                foreach (Unit unit in Tyr.Bot.Enemies())
                {
                    if (unit.IsFlying)
                        continue;

                    float newDist = SC2Util.DistanceSq(unit.Pos, agent.Unit.Pos);
                    if (newDist > targetDistance)
                        continue;

                    targetEnemy = unit;
                    targetDistance = newDist;
                }

                if (targetEnemy != null)
                {
                    agent.Order(2704, SC2Util.To2D(targetEnemy.Pos));
                    return true;
                }
            }

            if (closeEnemy != null)
            {
                if (distance >= 13 * 13)
                    agent.Order(Abilities.MOVE, target);
                else
                    agent.Order(Abilities.MOVE, agent.From(closeEnemy, 4));
                return true;
            }
            
            agent.Order(Abilities.MOVE, target);
            return true;
        }

        private void UpdateLocusts()
        {
            if (LocustUpdateFrame == Tyr.Bot.Frame)
                return;
            LocustUpdateFrame = Tyr.Bot.Frame;

            foreach (Agent agent in Tyr.Bot.UnitManager.Agents.Values)
                if (agent.Unit.UnitType == UnitTypes.LOCUST_FLYING && !SeenLocusts.Contains(agent.Unit.Tag))
                {
                    SeenLocusts.Add(agent.Unit.Tag);
                    LastLocustFrame = Tyr.Bot.Frame;
                }
        }
    }
}
