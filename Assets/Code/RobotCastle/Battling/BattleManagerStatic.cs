using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public partial class BattleManager
    {
        private static readonly List<Vector2Int> _coveredCells = new(20);
        private static readonly List<IHeroController> _heroesInRange = new(20);
        
        public static void ResetHeroAfterBattle(IHeroController hero)
        {
            hero.IsDead = false;
            var view = hero.View;
            view.stats.ClearDecorators();
            
            view.stats.Shield = 0f;
            view.heroUI.ShieldBar.Hide();
            view.stats.HealthReset.Reset(view);
            view.stats.ManaResetAfterBattle.Reset(view);
            view.heroUI.UpdateStatsView(view);
            
            view.healthManager.SetDamageable(false);
            view.state.Reset();
            view.gameObject.SetActive(true);
            view.heroUI.Show();
            view.processes.StopAll();
            hero.SetBehaviour(new HeroIdleBehaviour());
        }

        public static void PrepareForBattle(List<IHeroController> heroes)
        {
            var modsDb = ServiceLocator.Get<ModifiersDataBase>();
            foreach (var hero in heroes)
            {
                var view = hero.View;
                view.movement.SetupAgent();
                view.stats.ManaResetAfterBattle.Reset(view);
                view.heroUI.AssignStatsTracking(view);
                view.healthManager.SetDamageable(true);
                view.statAnimationSync.Init(true);
                foreach (var itemData in view.heroItemsContainer.Items)
                {
                    foreach (var id in itemData.modifierIds)
                    {
                        var mod = modsDb.GetModifier(id);
                        mod.AddToHero(view);
                    }
                }
            }
        }

        private class HeroAttackPoints
        {
            public IHeroController hero;
            public int points;
        }

        public static IHeroController GetBestTargetForAttack(IHeroController hero)
        {
            var map = hero.View.agent.Map;
            var myWorldPos = hero.View.transform.position;
            var myPos = map.GetCellPositionFromWorld(myWorldPos);
            // var cellsMask = hero.View.stats.Range.GetCellsMask();
            var enemies = hero.Battle.GetTeam(hero.TeamNum).enemyUnits;
            // var pointsData = new List<HeroAttackPoints>(enemies.Count);
            IHeroController bestTarget = null;
            var maxPoints = -float.MaxValue;
            foreach (var otherHero in enemies)
            {
                var enemyPos = otherHero.View.agent.CurrentCell;
                var d2 = (enemyPos - myPos).sqrMagnitude;
                var points = -d2 * 2f;
                if (otherHero.View.state.isStunned)
                    points++;
                if (!otherHero.View.state.isMoving)
                    points++;
                if (otherHero.View.state.attackData.CurrentEnemy == hero)
                    points += 2;
                var vec = otherHero.View.transform.position - myWorldPos;
                var angle = Mathf.Abs(Vector3.SignedAngle(vec, hero.View.transform.forward, Vector3.up));

                if (angle >= 180)
                    points -= 10;
                else if (angle >= 90)
                    points -= 8;
                else if (angle >= 10)
                    points -= 5;
                
                if (points >= maxPoints)
                {
                    maxPoints = points;
                    bestTarget = otherHero;
                }
            }
            if (bestTarget == null)
            {
                CLog.Log($"Error trying to calculate best enemy");
                return enemies[0];
            }
            return bestTarget;
        }

        
        public static IHeroController GetBestTargetForAttack2(IHeroController hero)
        {
            var map = hero.View.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.View.transform.position);
            var cellsMask = hero.View.stats.Range.GetCellsMask();
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
                        var h = enemy.View.stats.HealthCurrent.Get();
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