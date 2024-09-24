using System.Collections.Generic;
using NSubstitute.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public partial class BattleManager
    {
        private static readonly List<Vector2Int> _coveredCells = new(20);
        private static readonly List<IHeroController> _heroesInRange = new(20);
        
        public static void ResetHeroAfterBattle(IHeroController hero)
        {
            hero.SetIdle();
            hero.ResetForMerge();
        }

        public static IHeroController GetBestTargetForAttack(IHeroController hero)
        {
            var map = hero.View.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.View.transform.position);
            var cellsMask = hero.View.Stats.Range.GetCellsMask();
            _coveredCells.Clear();
            _heroesInRange.Clear();
            foreach (var val in cellsMask)
                _coveredCells.Add(myPos + val);
            var enemies = hero.Battle.GetTeam(hero.TeamNum).enemyUnits;
            var overallClosest = (IHeroController)null;
            var minD2 = int.MaxValue;
            var d2 = minD2;
            foreach (var otherHero in enemies)
            {
                var enemyPos = otherHero.View.agent.CurrentCell;
                d2 = (enemyPos - myPos).sqrMagnitude;
                if (d2 < minD2)
                {
                    minD2 = d2;
                    overallClosest = otherHero;
                }

                if (_coveredCells.Contains(enemyPos))
                    _heroesInRange.Add(otherHero);
            }

            switch (_heroesInRange.Count)
            {
                case 0:
                    return overallClosest;
                case 1:
                    return _heroesInRange[0];
                default:
                    var minHealth = float.MaxValue;
                    foreach (var enemy in _heroesInRange)
                    {
                        var h = enemy.View.Stats.HealthCurrent.Val;
                        if (h < minHealth)
                        {
                            minHealth = h;
                            overallClosest = enemy;
                        }
                    }

                    return overallClosest;
            }
        }

    }
}