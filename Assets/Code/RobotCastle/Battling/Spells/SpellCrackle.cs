using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellCrackle : Spell, IFullManaListener, IHeroProcess
    {
        public SpellCrackle(SpellConfigCrackle config, HeroComponents components)
        {
            _components = components;
            _config = config;
            Setup(config, out _manaAdder);
        }
        
        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
            Working(_token.Token);
        }
    
        public void Stop()
        {
            if (_isActive)
            {
                _token?.Cancel();
                _fx?.Hide();
                if (_isCasting)
                {
                    _components.animationEventReceiver.OnAttackEvent -= OnAnimEvent;       
                }
                _isCasting = false;
                _isActive = false;
            }
        }

        private CancellationTokenSource _token;
        private ConditionedManaAdder _manaAdder;
        private CrackleEffect _fx;
        private SpellConfigCrackle _config;
        private bool _isCasting;

        private async void Working(CancellationToken token)
        {
            _manaAdder.CanAdd = false;
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            _components.animator.Play("Cast",0,0);
            _components.animationEventReceiver.OnAttackEvent += OnAnimEvent;
            _isCasting = true;
            while (!token.IsCancellationRequested && _isCasting)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            DelayedResume(token);
            var fx = GetFxView();
            await fx.FlyAndHit(_components.projectileSpawnPoint.position, _config.damageBase[lvl], (int)_components.stats.SpellPower.Get(), 
                _config.bounces[lvl], hero, token);
            
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
            _manaAdder.CanAdd = false;
            _isActive = false;
        }

        private async void DelayedResume(CancellationToken token)
        {
            await HeroesManager.WaitGameTime(.5f, token);
            if (token.IsCancellationRequested || !_isActive) return;
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.ResumeCurrentBehaviour();
        }
        
        private void OnAnimEvent()
        {
            if (!_isActive) return;
            _isCasting = false;
        }
        
        private CrackleEffect GetFxView()
        {
            if (_fx != null) return _fx;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Crackle);
            var instance = UnityEngine.Object.Instantiate(prefab).GetComponent<CrackleEffect>();
            _fx = instance;
            return instance;
        }
        
    }
}