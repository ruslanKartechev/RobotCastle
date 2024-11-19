using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellRecklessProtection : Spell, IFullManaListener, IHeroProcess
    {
        public SpellRecklessProtection(SpellConfigRecklessProtection config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out _manaAdder);
        }
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _isActive = true;
            _components.processes.Add(this);
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _token?.Cancel();
                _isActive = false;
                _manaAdder.CanAdd = true;
            }
        }

        private SpellConfigRecklessProtection _config;
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesByLevel _fx;

        private async void Working(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            var hero = _components.gameObject.GetComponent<IHeroController>();
            var team = hero.Battle.GetTeam(hero.TeamNum);
            var map = _components.movement.Map;
            var units = new List<IHeroController>(team.ourUnits);
            
            Vector2Int targetCell = default;
            var target = FindTarget(units, hero, map, Vector2Int.up, out targetCell);
            if (target == null)
            {
                targetCell = _components.state.currentCell;
            }
            hero.PauseCurrentBehaviour();
            _components.movement.Stop();
            
            _components.animator.Play("Cast", 0,0);
            var lvl = (int)(HeroesManager.GetSpellTier(_components.stats.MergeTier));
            
            var fx = GetFxView();
            await _components.movement.JumpToCell(targetCell, token, _config.jumpTime, _config.jumpHeight);
            var mPos = _components.transform.position;
            fx.PlayLevelAtPoint(mPos, lvl);
            
            var range = _config.range[lvl];
            CellsMask mask = default;
            if (lvl % 2 == 0)
            {
                var atkRange = new AttackRangeRectangle(range, range);
                mask = new CellsMask(atkRange.GetCellsMask());
            }
            else
            {
                var atkRange = new AttackRangeRhombus(range);
                mask = new CellsMask(atkRange.GetCellsMask());
            }
            
            var enemies = HeroesManager.GetHeroesInsideCellMask(mask, mPos, map, team.enemyUnits);
            var damage = _config.damage[lvl] + _components.stats.SpellPower.Get();
            var args = new HeroDamageArgs(damage, EDamageType.Magical, _components);
            foreach (var enemy in enemies)
            {
                enemy.Components.damageReceiver.TakeDamage(args);
                var b = new HeroPushbackBehaviour(_config.pushbackDistance, _config.pushbackTime);
                enemy.SetBehaviour(b);
            }
            await Task.Delay(150, token);
            if (token.IsCancellationRequested) return;
            
            hero.ResumeCurrentBehaviour();
            _components.stats.ManaResetAfterFull.Reset(_components);
            Complete();
        }

        private void Complete()
        {
            _isActive = false;
            _manaAdder.CanAdd = true;
            _components.processes.Remove(this);

        }
        
        private SpellParticlesByLevel GetFxView()
        {
            if (_fx != null) return _fx;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_RecklessProtection);
            var instance = UnityEngine.Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            _fx = instance;
            return instance;
        }

        private IHeroController FindAnyPointAroundEnemy(List<IHeroController> options, IHeroController hero, 
            IMap map, out Vector2Int targetCell)
        {
            var team = hero.Battle.GetTeam(hero.TeamNum);
            foreach (var en in team.enemyUnits)
            {
                var c = en.Components.state.currentCell;
                var x = 0;
                var y = 1;
                for (x = -1; x <= 1; x++)
                {
                    var p = c + new Vector2Int(x,y);
                    if (map.IsFullyFree(p))
                    {
                        targetCell = p;
                        return en;
                    }
                }
                y = -1;
                for (x = -1; x <= 1; x++)
                {
                    var p = c + new Vector2Int(x,y);
                    if (map.IsFullyFree(p))
                    {
                        targetCell = p;
                        return en;
                    }
                }
                y = 0;
                x = 1;
                {
                    var p = c + new Vector2Int(x,y);
                    if (map.IsFullyFree(p))
                    {
                        targetCell = p;
                        return en;
                    }
                }
                x = -1;
                {
                    var p = c + new Vector2Int(x,y);
                    if (map.IsFullyFree(p))
                    {
                        targetCell = p;
                        return en;
                    }
                }
            }
            targetCell = default;
            return null;
        }
        
        private IHeroController FindTarget(List<IHeroController> options, IHeroController hero, IMap map, Vector2Int dir, out Vector2Int targetCell)
        {
            options.RemoveNulls();
            var myPos = _components.transform.position;
            var frw = _components.transform.forward;
            Comparison<IHeroController> compare = (a, b) =>
            {
                var v1 = (a.Components.transform.position - myPos);
                var d1 = Vector3.Dot(v1, frw);

                var v2 = (b.Components.transform.position - myPos);
                var d2 = Vector3.Dot(v2, frw);

                return d2.CompareTo(d1);
            };
            options.Sort(compare);
            
            IHeroController target = null;
            foreach (var tempUnit in options)
            {
                if (tempUnit.IsDead || tempUnit == hero)
                    continue;
                var coord = tempUnit.Components.state.currentCell + dir;
                if(!map.IsFullyFree(coord))
                    continue;
                targetCell = coord;
                target = tempUnit;
                return target;
            }
            targetCell = default;
            return target;
        }
        
    }
}