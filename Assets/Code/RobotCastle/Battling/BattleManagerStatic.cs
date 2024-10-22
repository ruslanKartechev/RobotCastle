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
            var components = hero.Components;
            components.stats.ClearDecorators();
            components.healthManager.ClearAllModifiers();
            
            components.heroUI.ShieldBar.Hide();
            components.stats.HealthReset.Reset(components);
            components.stats.ManaResetAfterBattle.Reset(components);
            components.heroUI.UpdateStatsView(components);
            
            components.healthManager.SetDamageable(false);
            components.state.Reset();
            components.gameObject.SetActive(true);
            components.heroUI.Show();
            components.processes.StopAll();
            
            components.damageSource.ClearDamageCalculationModifiers();
            components.damageSource.ClearPostDamageModifiers();
            foreach (var mod in components.preBattleRecurringMods)
                mod.Deactivate();
            
            hero.SetBehaviour(new HeroIdleBehaviour());
        }

        public static void PrepareForBattle(List<IHeroController> heroes)
        {
            foreach (var hero in heroes)
            {
                var components = hero.Components;
                components.movement.SetupAgent();
                components.stats.ManaResetAfterBattle.Reset(components);
                components.heroUI.AssignStatsTracking(components);
                components.healthManager.SetDamageable(true);
                components.statAnimationSync.Init(true);
                components.weaponsContainer.AddAllModifiersToHero(components);
                // foreach (var itemData in components.weaponsContainer.Items)
                // {
                //     foreach (var id in itemData.modifierIds)
                //     {
                //         var mod = modsDb.GetModifier(id);
                //         mod.AddToHero(components);
                //     }
                // }
                foreach (var mod in components.preBattleRecurringMods)
                    mod.Activate();
            }
        }

        public static IHeroController GetBestTargetForAttack(IHeroController hero)
        {
            var map = hero.Components.agent.Map;
            var myWorldPos = hero.Components.transform.position;
            var myPos = map.GetCellPositionFromWorld(myWorldPos);
            // var cellsMask = hero.View.stats.Range.GetCellsMask();
            var enemies = hero.Battle.GetTeam(hero.TeamNum).enemyUnits;
            // var pointsData = new List<HeroAttackPoints>(enemies.Count);
            IHeroController bestTarget = null;
            var maxPoints = -float.MaxValue;
            foreach (var otherHero in enemies)
            {
                var enemyPos = otherHero.Components.agent.CurrentCell;
                var d2 = (enemyPos - myPos).sqrMagnitude;
                var points = -d2 * 2f;
                if (otherHero.Components.state.isStunned)
                    points++;
                if (!otherHero.Components.state.isMoving)
                    points++;
                if (otherHero.Components.state.attackData.CurrentEnemy == hero)
                    points += 2;
                var vec = otherHero.Components.transform.position - myWorldPos;
                var angle = Mathf.Abs(Vector3.SignedAngle(vec, hero.Components.transform.forward, Vector3.up));

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
            var map = hero.Components.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.Components.transform.position);
            var cellsMask = hero.Components.stats.Range.GetCellsMask();
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
                var enemyPos = otherHero.Components.agent.CurrentCell;
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
                        var h = enemy.Components.stats.HealthCurrent.Get();
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