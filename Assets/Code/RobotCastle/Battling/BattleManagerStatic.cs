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
        
        public static void PrepareForBattle(IHeroController hero)
        {
            var mods = ServiceLocator.Get<ModifiersDataBase>();
            var view = hero.View;
            view.movement.SetupAgent();
            view.stats.ManaResetAfterBattle.Reset(view);
            view.heroUI.AssignStatsTracking(view);
            view.healthManager.SetDamageable(true);
            foreach (var itemData in view.heroItemsContainer.Items)
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