﻿using System;
using System.Collections.Generic;
using SC2APIProtocol;
using Tyr.Agents;
using Tyr.Managers;
using Tyr.MapAnalysis;
using Tyr.Tasks;
using Tyr.Util;

namespace Tyr.BuildingPlacement
{
    /*
     * This class is responsible for managing the placement of buildings.
     */
    public class WarpInPlacer
    {   
        public static Point2D FindPlacement(Point2D target, uint type)
        {
            Point2D result = findPlacementLocal(target, type, 20);
            return result;
        }

        private static Point2D findPlacementLocal(Point2D target, uint type, int maxDist)
        {
            target = SC2Util.Point((int)target.X, (int)target.Y);

            for (int range = 0; range < maxDist; range++)
            {
                for (int x = -range; x <= range; x++)
                {
                    if (CheckPlacement(SC2Util.Point(target.X + x, target.Y - range), type, null, false))
                        return SC2Util.Point(target.X + x, target.Y - range);
                    if (CheckPlacement(SC2Util.Point(target.X + x, target.Y + range), type, null, false))
                        return SC2Util.Point(target.X + x, target.Y + range);
                }
                for (int y = -range + 1; y <= range - 1; y++)
                {
                    if (CheckPlacement(SC2Util.Point(target.X + range, target.Y + y), type, null, false))
                        return SC2Util.Point(target.X + range, target.Y + y);
                    if (CheckPlacement(SC2Util.Point(target.X - range, target.Y + y), type, null, false))
                        return SC2Util.Point(target.X - range, target.Y + y);
                }
            }
            // No placement found.
            return null;
        }

        public static bool CheckPlacement(Point2D location, uint type, BuildRequest skipRequest, bool buildingsOnly)
        {
            // Check if the building can be placed on this position of the map.
            if (!SC2Util.GetTilePlacable((int)Math.Round(location.X), (int)Math.Round(location.Y)))
                return false;

            foreach (Unit unit in Tyr.Bot.Observation.Observation.RawData.Units)
                if (!unit.IsFlying && !CheckDistance(location, type, SC2Util.To2D(unit.Pos), unit.UnitType, buildingsOnly))
                    return false;

            foreach (BuildRequest request in ConstructionTask.Task.UnassignedRequests)
                if (request != skipRequest && !CheckDistance(location, type, request.Pos, request.Type, buildingsOnly))
                    return false;

            foreach (BuildRequest request in ConstructionTask.Task.BuildRequests)
                if (request != skipRequest && !CheckDistance(location, type, request.Pos, request.Type, buildingsOnly))
                    return false;

            foreach (Unit unit in Tyr.Bot.Observation.Observation.RawData.Units)
            {
                if (unit.UnitType != UnitTypes.PYLON || unit.BuildProgress < 1)
                    continue;

                if (Tyr.Bot.MapAnalyzer.MapHeight((int)unit.Pos.X, (int)unit.Pos.Y) < Tyr.Bot.MapAnalyzer.MapHeight((int)location.X, (int)location.Y))
                    continue;

                if (location.X - 1 >= unit.Pos.X - 6 && location.X + 1 <= unit.Pos.X + 6
                    && location.Y - 1 >= unit.Pos.Y - 7 && location.Y + 1 <= unit.Pos.Y + 7)
                {
                    if (SC2Util.DistanceGrid(unit.Pos, location) <= 10 - 1)
                        return true;
                }
            }
            return false;
        }

        public static bool CheckDistance(Point2D location, uint buildingType, Point2D unitPos, uint unitType, bool buildingsOnly)
        {
            if (buildingsOnly && !BuildingType.LookUp.ContainsKey(unitType))
                return true;

            if (unitType == UnitTypes.ADEPT_PHASE_SHIFT
                || unitType == UnitTypes.KD8_CHARGE)
                return true;
            
            if (UnitTypes.CombatUnitTypes.Contains(unitType))
                return SC2Util.DistanceGrid(unitPos, location) > 1;
            if (UnitTypes.WorkerTypes.Contains(unitType))
                return SC2Util.DistanceGrid(unitPos, location) > 3;
            
            return CheckDistanceClose(location, buildingType, unitPos, unitType);
        }

        public static bool CheckDistanceClose(Point2D location, uint buildingType, Point2D unitPos, uint unitType)
        {
            float dx = 0.5f + (BuildingType.LookUp.ContainsKey(unitType) ? BuildingType.LookUp[unitType].Size.X / 2f: 1.2f) - 0.1f;
            float dy = 0.5f + (BuildingType.LookUp.ContainsKey(unitType) ? BuildingType.LookUp[unitType].Size.Y / 2f : 1.2f) - 0.1f;
            
            return Math.Abs(location.X - unitPos.X) >= dx || Math.Abs(location.Y - unitPos.Y) >= dy;
        }

        public static bool CanHaveAddOn(uint type)
        {
            return type == UnitTypes.BARRACKS || type == UnitTypes.FACTORY || type == UnitTypes.STARPORT;
        }
    }
}
