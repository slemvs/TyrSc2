﻿using Tyr.Agents;

namespace Tyr.StrategyAnalysis
{
    public class SpineCrawler : Strategy
    {
        private static SpineCrawler Singleton = new SpineCrawler();

        public static Strategy Get()
        {
            return Singleton;
        }

        public override bool Detect()
        {
            return Tyr.Bot.EnemyStrategyAnalyzer.Count(UnitTypes.SPINE_CRAWLER) + Tyr.Bot.EnemyStrategyAnalyzer.Count(UnitTypes.SPINE_CRAWLER_UPROOTED) > 0;
        }

        public override string Name()
        {
            return "SpineCrawler";
        }
    }
}
