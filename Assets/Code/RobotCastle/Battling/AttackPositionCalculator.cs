using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackPositionCalculator
    {
        public List<IMovingUnit> units = new(10);

        public void AddUnit(IMovingUnit unit)
        {
            if(units.Contains(unit) == false)
                units.Add(unit);
        }

        public void RemoveUnit(IMovingUnit unit)
        {
            units.Remove(unit);
        }

        public bool CheckIfAnyUnitAssignedWithCell(Vector2Int cell)
        {
            foreach (var unit in units)
            {
                if (unit.TargetCell == cell)
                    return true;
            }
            return false;
        }
        
        public bool CheckIfAnyUnitAssignedWithCellExcept(Vector2Int cell, IMovingUnit exceptionUnit)
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
        /// <returns>True if should me. False if already on the cell. out targetPosition - position to move to</returns>
        public bool GetPositionToAttack(HeroController hero, HeroController enemy, out Vector2Int targetCell)
        {
            var map = hero.HeroView.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.transform.position);
            var cellsMask = hero.HeroView.Stats.Range.GetCellsMask();
            var coveredCells = new List<Vector2Int>(cellsMask.Count);
            foreach (var val in cellsMask)
                coveredCells.Add(myPos + val);
            var enemyPos = map.GetCellPositionFromWorld(enemy.transform.position);
            if (coveredCells.Contains(enemyPos))
            {
                targetCell = enemyPos;
                return false; // don't move, just attack
            }
            // cells around the enemy
            coveredCells.Clear();
            var otherUnitsPositions = new List<Vector2Int>(10);
            foreach (var agent in map.ActiveAgents)
            {
                if(agent != hero.HeroView.agent)
                    otherUnitsPositions.Add(agent.CurrentCell);
            }
            foreach (var dir in cellsMask)
            {
                var nextPos = enemyPos + dir;
                if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x >= map.Size.x || nextPos.y >= map.Size.y)
                    continue;
                if (!otherUnitsPositions.Contains(nextPos) && !CheckIfAnyUnitAssignedWithCellExcept(nextPos, hero.HeroView.movement))
                    coveredCells.Add(nextPos);
            }

            if (coveredCells.Count == 0)
            {
                CLog.LogRed($"[{nameof(AttackPositionCalculator)}] Error. No suitable covered cells");
                targetCell = myPos;
                return true;
            }
            targetCell = myPos;
            var minDist = int.MaxValue;
            var dist = minDist;
            var options = new List<Vector2Int>(4);
            foreach (var cell in coveredCells)
            {
                dist = (cell - myPos).sqrMagnitude;
                if (dist == minDist)
                {
                    options.Add(cell);
                }
                else if (dist < minDist)
                {
                    minDist = dist;
                    options.Add(cell);
                }
            }
            targetCell = options.Random();
            return true;
        }
        
    }
}