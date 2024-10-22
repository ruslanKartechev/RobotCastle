using System.Collections.Generic;
using Bomber;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public partial class BattleManager
    {
        public static void SetClosestAvailableDesiredPositions(List<SpawnMergeItemArgs> args, Vector2Int center)
        {
            var map = ServiceLocator.Get<IMap>();
            var battle = ServiceLocator.Get<BattleManager>()._battle;
            var size = map.Size;
            var busyCells = new List<Vector2Int>(30);
            foreach (var en in battle.enemiesAlive)
            {
                busyCells.Add(en.Components.state.currentCell);
                busyCells.Add(en.Components.state.targetMoveCell);
            }
            foreach (var en in battle.playersAlive)
            {
                busyCells.Add(en.Components.state.currentCell);
                busyCells.Add(en.Components.state.targetMoveCell);
            }
            var radius = 1;
            for(var i = 0; i < args.Count; i++)
            {
                radius = 1;
                var didFind = false;
                for (var x = -radius; x <= radius && !didFind; x++)
                {
                    for (var y = -radius; y <= radius && !didFind; y++)
                    {
                        if (x == 0 && y == 0) 
                            continue;
                        
                        var cellPos = center + new Vector2Int(x, y);
                        if (map.IsOutOfBounce(cellPos))
                            continue;
                        if (busyCells.Contains(cellPos))
                            continue;
                        args[i].preferredCoordinated = cellPos;
                        busyCells.Add(cellPos);
                        didFind = true;
                    }
                }
                radius++;
                if (radius >= size.x)
                    break;
            }
        }
        
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

        
    
    }
}