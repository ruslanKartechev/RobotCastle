using System.Collections.Generic;
using RobotCastle.Core;
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
            view.Stats.ClearDecorators();
            
            view.Stats.HealthReset.Reset(view);
            view.Stats.ManaReset.Reset(view);
            view.heroUI.UpdateStatsView(view);
            
            view.HealthManager.SetDamageable(false);
            view.AttackData.Reset();
            view.gameObject.SetActive(true);
            view.heroUI.Show();
            hero.SetBehaviour(new HeroIdleBehaviour());
        }

        public static void PrepareForBattle(List<IHeroController> heroes)
        {
            var modsDb = ServiceLocator.Get<ModifiersDataBase>();
            foreach (var hero in heroes)
            {
                var view = hero.View;
                view.movement.SetupAgent();
                view.Stats.ManaReset.Reset(view);
                view.heroUI.AssignStatsTracking(view);
                view.HealthManager.SetDamageable(true);
                foreach (var itemData in view.HeroItemsContainer.Items)
                {
                    foreach (var id in itemData.modifierIds)
                    {
                        var mod = modsDb.GetModifier(id);
                        mod.AddToHero(view);
                    }
                }
            }
        }
        
        public static void PrepareForBattle(IHeroController hero)
        {
            var mods = ServiceLocator.Get<ModifiersDataBase>();
            var view = hero.View;
            view.movement.SetupAgent();
            view.Stats.ManaReset.Reset(view);
            view.heroUI.AssignStatsTracking(view);
            view.HealthManager.SetDamageable(true);
            foreach (var itemData in view.HeroItemsContainer.Items)
            {
                foreach (var id in itemData.modifierIds)
                {
                    var mod = mods.GetModifier(id);
                    mod.AddToHero(view);
                }
            }
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