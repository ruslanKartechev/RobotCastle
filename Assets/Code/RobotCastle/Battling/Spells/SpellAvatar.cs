﻿using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Utils;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellAvatar :  Spell, IFullManaListener, IHeroProcess, IStatDecorator
    {
        public SpellAvatar(SpellConfigAvatar config, HeroComponents components)
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
                _components.attackManager.OnAttackStep += OnAttackStep;
                _manaAdder.CanAdd = true;
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
        private float _def;
        private int _lvl;

        private async void Working(CancellationToken token)
        {
            _isActive = true;
            _manaAdder.CanAdd = false;
            var fx = GetFxView();
            fx.ShowUntilOff(_components.transform);
            _lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            
            _def = _config.defByTier[_lvl];
            _components.attackManager.OnAttackStep += OnAttackStep;
            _components.stats.MagicalResist.AddDecorator(this);
            _components.stats.PhysicalResist.AddDecorator(this);
            
            await Task.Delay(_config.duration.SecToMs(), token);
            
            _components.stats.MagicalResist.RemoveDecorator(this);
            _components.stats.PhysicalResist.RemoveDecorator(this);
            
            _manaAdder.CanAdd = true;
            _isActive = false;
            fx.gameObject.SetActive(false);
            _components.stats.ManaResetAfterFull.Reset(_components);
        }

        private void OnAttackStep()
        {
            var damage = _config.spellDamage;
            var range = _config.cellsMasksByTear[_lvl];
            var allEnemies = HeroesManager.GetHeroesEnemies(_components);
            var map = _components.agent.Map;
            var affectedEnemies = HeroesManager.GetHeroesInsideCellMask(range, _components.transform.position, map, allEnemies);
            foreach (var hero in affectedEnemies)
            {
                hero.Components.damageReceiver
                    .TakeDamage(new HeroDamageArgs(damage, EDamageType.Magical, _components));
            }
            _fxView.PlayHitParticles(_lvl);
        }

        private SpellParticlesOnHero GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Avatar);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }



    }
}