﻿
using Tyr.Agents;

namespace Tyr.StrategyAnalysis
{
    public class Queen : Strategy
    {
        private static Queen Singleton = new Queen();

        public static Strategy Get()
        {
            return Singleton;
        }

        public override bool Detect()
        {
            return Tyr.Bot.EnemyStrategyAnalyzer.Count(UnitTypes.QUEEN) > 0;
        }

        public override string Name()
        {
            return "Queen";
        }
    }
}
