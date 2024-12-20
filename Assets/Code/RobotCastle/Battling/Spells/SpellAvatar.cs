﻿using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Utils;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellAvatar :  Spell, IFullManaListener, IHeroProcess, IStatDecorator
    {
        public SpellAvatar(SpellConfigAvatar config, HeroComponents components, string fxId)
        {
            _config = config;
            _components = components;
            _fxId = fxId;
            Setup(config, out _manaAdder);
            _spDecor = new SimpleDecoratorAdd(_config.spellDamage);
            _components.stats.SpellPower.AddPermanentDecorator(_spDecor);
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
                _components.attackManager.OnAttackStep += OnAttackStep;
                _manaAdder.CanAdd = true;
                if(_fxView != null)
                    _fxView.gameObject.SetActive(false);
                _components.stats.MagicalResist.RemoveDecorator(this);
                _components.stats.PhysicalResist.RemoveDecorator(this);
                _token.Cancel();
                
            }
        }
        
        public string name => "avatar";
        public int order => 10;
        public float Decorate(float val) => val + _def;

        private CancellationTokenSource _token;
        private SpellConfigAvatar _config;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesOnHero _fxView;
        private SimpleDecoratorAdd _spDecor;
        private float _def;
        private int _lvl;
        private string _fxId;

        private async void Working(CancellationToken token)
        {
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.processes.Add(this);
            var fx = GetFxView();
            fx.ShowUntilOff(_components.transform);
            _lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            
            _def = _config.defByTier[_lvl];
            _components.attackManager.OnAttackStep += OnAttackStep;
            _components.stats.MagicalResist.AddDecorator(this);
            _components.stats.PhysicalResist.AddDecorator(this);
            
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, _config.duration);
            await HeroesManager.WaitGameTime(_config.duration, token);
            if (token.IsCancellationRequested) 
                return;
            _components.attackManager.OnAttackStep -= OnAttackStep;
            _components.processes.Remove(this);

            _components.stats.MagicalResist.RemoveDecorator(this);
            _components.stats.PhysicalResist.RemoveDecorator(this);
            
            _manaAdder.CanAdd = true;
            _isActive = false;
            fx.gameObject.SetActive(false);
            _components.stats.ManaResetAfterFull.Reset(_components);
        }

        private void OnAttackStep()
        {
            var range = _config.cellsMasksByTear[_lvl];
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            var map = _components.movement.Map;
            var affectedEnemies = HeroesManager.GetHeroesInsideCellMask(range, _components.transform.position, map, allEnemies);
            foreach (var hero in affectedEnemies)
            {
                _components.damageSource.DamageSpell(hero.Components.damageReceiver);
            }
            _fxView.PlayHitParticles(_lvl);
        }

        private SpellParticlesOnHero GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(_fxId);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }

    }
}