﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Utils;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellPowerOfRock : Spell, IFullManaListener, IHeroProcess, IStatDecorator
    { 
        public SpellPowerOfRock(SpellConfigPowerOfRock config, HeroComponents components)
        {
            _config = config;
            _components = components;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
        }

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _manaAdder.CanAdd = true;
                _components.attackManager.OnAttackStep -= OnAttack;
                _components.stats.PhysicalResist.RemoveDecorator(this);
                _components.stats.MagicalResist.RemoveDecorator(this);
                _token?.Cancel();
            }
        }
        
        public string name => "power_of_rock";

        public int order => 10;
        
        public float Decorate(float val) => val + _def;
        
        private SpellConfigPowerOfRock _config;
        private HeroComponents _components;
        private CancellationTokenSource _token;
        private SpellParticleOnGridEffect _fxView;
        private ConditionedManaAdder _manaAdder;
        private float _def;

        private async void Working(CancellationToken token)
        {
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.processes.Add(this);

            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var mask = _config.maskByTier[lvl];
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            var map = _components.agent.Map;
            _def = _config.defByTier[lvl];
            const int waitMs = (int)(.25f * 1000);
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            const int frameInside = 5;
            
            List<IHeroController> affectedEnemies = null;
            var didFind = false;
            var rotatedMask = new CellsMask(){
                mask = new List<Vector2Int>(mask.mask.Count)
            };
            
            while (!token.IsCancellationRequested && !didFind)
            {
                rotatedMask.SetAsRotated(mask, _components.transform.rotation);
                affectedEnemies = HeroesManager.GetHeroesInsideCellMask(rotatedMask, _components.transform.position, map, allEnemies);
                if (affectedEnemies.Count > 0)
                {
                    for (var i = 0; i < frameInside; i++)
                        await Task.Yield();
                    if (token.IsCancellationRequested) return;
                    rotatedMask.SetAsRotated(mask, _components.transform.rotation);
                    affectedEnemies = HeroesManager.GetHeroesInsideCellMask(rotatedMask, _components.transform.position, map, allEnemies);
                    if(affectedEnemies.Count > 0)
                        didFind = true;
                }
                else
                    await Task.Delay(waitMs, token);
            }

            if (token.IsCancellationRequested) return;

            if (affectedEnemies is { Count: > 0 })
            {
                var fx = GetFxView();
                var worldPositions = new List<Vector3>(affectedEnemies.Count);
                
                var centerCell = _components.state.currentCell;
                foreach (var dir in rotatedMask.mask)
                    worldPositions.Add(map.GetWorldFromCell(centerCell + dir));
                    
                fx.Show(worldPositions);
                var damageArgs = new HeroDamageArgs(_config.hitDamage, EDamageType.Magical, _components);
                foreach (var en in affectedEnemies)
                    en.Components.damageReceiver.TakeDamage(damageArgs);

                _components.attackManager.OnAttackStep += OnAttack;
                _components.stats.PhysicalResist.AddDecorator(this);
                _components.stats.MagicalResist.AddDecorator(this);
                
                await Task.Delay(_config.auraDuration.SecToMs());
                if (token.IsCancellationRequested)
                    return;
                _components.attackManager.OnAttackStep -= OnAttack;
                _components.stats.PhysicalResist.RemoveDecorator(this);
                _components.stats.MagicalResist.RemoveDecorator(this);
                _components.stats.ManaResetAfterFull.Reset(_components);
                _components.processes.Remove(this);
                _manaAdder.CanAdd = true;
                _isActive = false;
            }
            else
            {
                _components.stats.ManaResetAfterFull.Reset(_components);
                _components.processes.Remove(this);
                Stop();
            }
        }

        private void OnAttack()
        {
            if (_components.attackManager.LastTarget == null)
                return;
            _components.attackManager.LastTarget
                .TakeDamage(new HeroDamageArgs(_config.addedDamageOnAttacks, EDamageType.Magical, _components));
        }
        
        private SpellParticleOnGridEffect GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_PowerOfRock);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticleOnGridEffect>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }


    }
}