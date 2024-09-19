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

        public bool AnyEqual(Vector2Int cell)
        {
            foreach (var unit in units)
            {
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
            // cells around the nemy now
            coveredCells.Clear();
            var otherUnits = new List<Vector2Int>(10);
            foreach (var agent in map.ActiveAgents)
            {
                if(agent != hero.HeroView.agent)
                    otherUnits.Add(agent.CurrentCell);
            }
            foreach (var val in cellsMask)
            {
                var nc = enemyPos + val;
                // if(AnyEqual(nc))
                    // CLog.Log($"{nc.ToString()} is marked already");
                // if(otherUnits.Contains(nc))
                    // CLog.Log($"{nc.ToString()} occupied");
                if (!otherUnits.Contains(nc) && !AnyEqual(nc))
                {
                    coveredCells.Add(nc);
                }
            }
            targetCell = myPos;
            var minD = int.MaxValue;
            var d = minD;
            foreach (var cell in coveredCells)
            {
                d = (cell - myPos).sqrMagnitude;
                if (d < minD)
                {
                    minD = d;
                    targetCell = cell;
                }
            }
            return true;
        }
        
    }
}