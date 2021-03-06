﻿using SC2APIProtocol;
using Tyr.Agents;
using Tyr.Util;

namespace Tyr.Micro
{
    public class HellbatController : CustomController
    {
        public override bool DetermineAction(Agent agent, Point2D target)
        {
            if (agent.Unit.UnitType != UnitTypes.HELLBAT)
                return false;

            float dist;
            if (agent.DistanceSq(Tyr.Bot.MapAnalyzer.StartLocation) >= 40 * 40)
            {
                Point2D retreatTo = null;
                dist = 15 * 15;
                bool sieged = false;
                foreach (Agent ally in Tyr.Bot.UnitManager.Agents.Values)
                {
                    if (ally.Unit.UnitType != UnitTypes.SIEGE_TANK
                        && ally.Unit.UnitType != UnitTypes.SIEGE_TANK_SIEGED)
                        continue;

                    float newDist = agent.DistanceSq(ally);
                    if (newDist < dist)
                    {
                        retreatTo = SC2Util.To2D(ally.Unit.Pos);
                        dist = newDist;
                        sieged = ally.Unit.UnitType == UnitTypes.SIEGE_TANK_SIEGED;
                    }
                }
                if (retreatTo != null && dist >= (sieged ? 10 * 10 : 5 * 5))
                {
                    agent.Order(Abilities.MOVE, retreatTo);
                    return true;
                }
            }

            return false;
        }
    }
}
