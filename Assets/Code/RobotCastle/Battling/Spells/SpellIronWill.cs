﻿using System.Threading;
using SleepDev;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
    
namespace RobotCastle.Battling
{
    public class SpellIronWill : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public float BaseSpellPower => _config.spellResist[(int)HeroesManager.GetSpellTier(_components.stats.MergeTier)];
        
        public string name => "spell";
        
        public int order => 10;
        
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellIronWill(HeroComponents view, SpellConfigIronWill config)
        {
            _components = view;
            _config = config;
            Setup(config, out _manaAdder);
            _components.stats.SpellPower.AddPermanentDecorator(this);
            _modShield = new DamageTakeModShield(0f, view);
        }
        
        public void Stop()
        {
            if (!_isActive) return;
            _isActive = false;
            if(_fx != null)
                _fx.gameObject.SetActive(false);
            if (_isCasting)
            {
                _components.animationEventReceiver.OnAttackEvent -= OnAnimEvent;
                _isCasting = false;
            }
        }

        public void OnFullMana(GameObject heroGo)
        {
            // CLog.Log($"[{_components.gameObject.name}] [{nameof(SpellIronWill)}] Adding shield bonus");
            _components.processes.Add(this);
            _components.stats.ManaCurrent.Val = 0;
            _manaAdder.CanAdd = false;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _isActive = true;
            CastAnimated(_token.Token);
        }

        
        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesOnHero _fx;
        private SpellConfigIronWill _config;
        private DamageTakeModShield _modShield;
        private bool _isCasting;
        
        
        private void AddShield()
        {
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            _modShield.AddToHero(_config.physResist[lvl]);
            
            _components.heroUI.ShieldBar.TrackUntilZero(_modShield);
            _fx = GetFxView();
            _fx.ShowUntilOff(_components.transform);
        }

        private async void CastAnimated(CancellationToken token)
        {
            const float afterCastDelay = .4f;
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            await Task.Yield();
            _isCasting = true;
            _components.animator.Play("Cast");
            _components.animationEventReceiver.OnAttackEvent += OnAnimEvent;
            while (_isCasting && !token.IsCancellationRequested)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            _components.animationEventReceiver.OnAttackEvent -= OnAnimEvent;

            _isCasting = false;
            AddShield();
            await HeroesManager.WaitGameTime(afterCastDelay, token);
            if (token.IsCancellationRequested)
                return;

            hero.ResumeCurrentBehaviour();
            _manaAdder.CanAdd = true;
            while (!token.IsCancellationRequested && _modShield.Get() > 0)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            _fx.gameObject.SetActive(false);
            _isActive = false;
        }

        private void OnAnimEvent()
        {
            if (!_isActive) return;
            _isCasting = false;
            if (_components.spellSounds.Count > 0)
                _components.spellSounds[0].Play();
        }

        private SpellParticlesOnHero GetFxView()
        {
            if (_fx != null) return _fx;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_IronWill);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            _fx = instance;
            return instance;
        }

        
    }
}