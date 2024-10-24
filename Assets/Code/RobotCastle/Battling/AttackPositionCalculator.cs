﻿using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackPositionCalculator
    {
        public List<IMoveInfoProvider> units = new(10);

        public void AddUnit(IMoveInfoProvider unit)
        {
            if(units.Contains(unit) == false)
                units.Add(unit);
        }

        public void RemoveUnit(IMoveInfoProvider unit)
        {
            units.Remove(unit);
        }

        public bool CheckIfAnyUnitHasThisTargetPosition(Vector2Int cell, IMoveInfoProvider exceptionUnit)
        {
            foreach (var unit in units)
            {
                if(unit == exceptionUnit)
                    continue;
                if (unit.TargetCell == cell)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="hero">Your hero</param>
        /// <param name="enemy">Enemy hero to get to</param>
        /// <returns>True if should move. False if already on the cell. out targetPosition - position to move to</returns>
        public bool GetPositionToAttack(IHeroController hero, 
            IHeroController enemy, 
            out Vector2Int targetCell, 
            out int distance)
        {
            var map = hero.Components.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.Components.transform.position);
            var cellsMask = hero.Components.stats.Range.GetCellsMask();
            var coveredCells = new List<Vector2Int>(cellsMask.Count);
            foreach (var val in cellsMask)
                coveredCells.Add(myPos + val);
            var enemyPos = map.GetCellPositionFromWorld(enemy.Components.transform.position);
            if (coveredCells.Contains(enemyPos))
            {
                targetCell = enemyPos;
                distance = (enemyPos - myPos).sqrMagnitude;
                return false; // don't move, just attack
            }
            var otherUnitsPositions = new List<Vector2Int>(10);
            foreach (var agent in map.ActiveAgents)
            {
                if(agent != hero.Components.agent)
                    otherUnitsPositions.Add(agent.CurrentCell);
            }
            coveredCells.Clear();
            foreach (var dir in cellsMask)
            {
                var nextPos = enemyPos + dir;
                if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x >= map.Size.x || nextPos.y >= map.Size.y)
                    continue;
                if (!otherUnitsPositions.Contains(nextPos) && !CheckIfAnyUnitHasThisTargetPosition(nextPos, hero.Components.state))
                    coveredCells.Add(nextPos);
            }

            if (coveredCells.Count == 0)
            {
                CLog.Log($"[{nameof(AttackPositionCalculator)}] Error. No suitable covered cells");
                targetCell = myPos;
                distance = 0;
                return true;
            }
            targetCell = myPos;
            var minDist = int.MaxValue;
            var dist = minDist;
            // var options = new List<Vector2Int>(4);
            foreach (var cell in coveredCells)
            {
                dist = (cell - myPos).sqrMagnitude;
                if (dist == minDist)
                {
                    // options.Add(cell);
                }
                else if (dist < minDist)
                {
                    minDist = dist;
                    // options.Add(cell);
                    targetCell = cell;
                }
            }
            // targetCell = options.Random();
            distance = (targetCell - myPos).sqrMagnitude;
            return true;
        }

    }
}