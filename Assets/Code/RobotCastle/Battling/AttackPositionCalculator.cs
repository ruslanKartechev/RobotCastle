using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    // The algorithm
    // Find the closest enemy to attack
    // Find shortest path to the enemy
    // Make 1 cell movement
    // Check if the best enemy is the same enemy
    // If can attack - attack
    // If not - keep moving
    
    
    public class AttackPositionCalculator
    {
        public List<HeroStateData> units = new(10);

        public void AddUnit(HeroStateData unit)
        {
            if(units.Contains(unit) == false)
                units.Add(unit);
        }

        public void RemoveUnit(HeroStateData unit)
        {
            units.Remove(unit);
        }

        public bool CheckIfAnyUnitHasThisTargetPosition(Vector2Int cell, HeroStateData exceptionUnit, HeroStateData exceptionUnit2)
        {
            foreach (var unit in units)
            {
                if(unit == exceptionUnit || unit == exceptionUnit2)
                    continue;
                if (unit.TargetCell == cell)
                    return true;
            }
            return false;
        }

        public List<Vector2Int> GetPossibleCellsToAttackEnemy(IHeroController hero, IHeroController enemy)
        {
            var map = hero.Components.movement.Map;
            var myPos = map.GetCellPositionFromWorld(hero.Components.transform.position);
            var cellsMask = hero.Components.stats.Range.GetCellsMask();
            var coveredCells = new List<Vector2Int>(cellsMask.Count);
            var otherUnitsPositions = new List<Vector2Int>(10);
            foreach (var agent in map.ActiveAgents)
            {
                if (agent != hero.Components.movement)
                    otherUnitsPositions.Add(agent.CurrentCell);
            }
            var enemyPos = enemy.Components.state.currentCell;
            foreach (var dir in cellsMask)
            {
                var nextPos = enemyPos + dir;
                if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x >= map.Size.x || nextPos.y >= map.Size.y)
                    continue;
                if (!otherUnitsPositions.Contains(nextPos) 
                    && !CheckIfAnyUnitHasThisTargetPosition(nextPos, hero.Components.state, enemy.Components.state))
                    coveredCells.Add(nextPos);
            }

            return coveredCells;
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
            var map = hero.Components.movement.Map;
            var myPos = map.GetCellPositionFromWorld(hero.Components.transform.position);
            var cellsMask = hero.Components.stats.Range.GetCellsMask();
            var coveredCells = new List<Vector2Int>(cellsMask.Count);
            foreach (var val in cellsMask)
                coveredCells.Add(myPos + val);
            var enemyPos = enemy.Components.state.currentCell;
            if (coveredCells.Contains(enemyPos))
            {
                targetCell = enemyPos;
                distance = (enemyPos - myPos).sqrMagnitude;
                return false; // don't move, just attack or rotate
            }
            var otherUnitsPositions = new List<Vector2Int>(10);
            foreach (var agent in map.ActiveAgents)
            {
                if(agent != hero.Components.movement)
                    otherUnitsPositions.Add(agent.CurrentCell);
            }
            coveredCells.Clear();
            foreach (var dir in cellsMask)
            {
                var nextPos = enemyPos + dir;
                if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x >= map.Size.x || nextPos.y >= map.Size.y)
                    continue;
                if (!otherUnitsPositions.Contains(nextPos) && !CheckIfAnyUnitHasThisTargetPosition(nextPos, hero.Components.state, enemy.Components.state))
                    coveredCells.Add(nextPos);
            }
            
            if (coveredCells.Count == 0)
            {
                CLog.Log($"[{nameof(AttackPositionCalculator)}] Error. No suitable cells around enemy. Hero: {hero.Components.gameObject.name}");
                targetCell = myPos;
                distance = 0;
                return true;
            }
            targetCell = myPos;
            var minDist = int.MaxValue;
            var dist = minDist;
            foreach (var cell in coveredCells)
            {
                dist = (cell - myPos).sqrMagnitude;
                if (dist <= minDist)
                {
                    minDist = dist;
                    targetCell = cell;
                }
            }
            distance = (targetCell - myPos).sqrMagnitude;
            return true;
        }

    }
}