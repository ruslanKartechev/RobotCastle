using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellTerrorize : Spell, IFullManaListener, IHeroProcess
    {
        public SpellTerrorize(SpellConfigTerrorize config, HeroComponents components)
        {
            _config = config;
            _components = components;
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
                _isActive = false;
                if (_isCasting)
                {
                    _isCasting = false;
                    _components.animationEventReceiver.OnAttackEvent += Cast;
                }
            }
        }

        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;
        private SpellConfigTerrorize _config;
        private bool _isCasting;
        private SpellParticlesByLevel _fx;

        private async void Working(CancellationToken token)
        {
            _manaAdder.CanAdd = false;
            var hasEnemiesInRange = false;
            do
            {
                GetEnemiesInRange();
                hasEnemiesInRange = _enemiesInRange.Count > 0;
                await Task.Yield();
                if (token.IsCancellationRequested) return; 
                
            } while (!token.IsCancellationRequested && (!hasEnemiesInRange || !_components.state.isAttacking));
            if (token.IsCancellationRequested) return;
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            _components.animationEventReceiver.OnAttackEvent += Cast;
            _isCasting = true;
            _components.animator.Play("Cast", 0, 0);
            while(!token.IsCancellationRequested && _isCasting)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            await HeroesManager.WaitGameTime(_config.resumeDelay, token);
            if (token.IsCancellationRequested) return;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _manaAdder.CanAdd = true;
            _isActive = false;
            hero.ResumeCurrentBehaviour();
            _components.processes.Remove(this);
        }

        private List<IHeroController> _enemiesInRange = new(5);
        private void Cast()
        {
            _isCasting = false;
            _components.animationEventReceiver.OnAttackEvent -= Cast;
            var fx = GetFxView();
            fx.PlayAtPoint(_components.transform, 0);
            GetEnemiesInRange();
            foreach (var hh in _enemiesInRange)
                hh.SetBehaviour(new HeroFleeBehaviour(_config.fleeTime));
        }

        private void GetEnemiesInRange()
        {
            _enemiesInRange.Clear();
            var enemies = HeroesManager.GetHeroesEnemies(_components);
            var mPos = _components.state.currentCell;
            foreach (var hh in enemies)
            {
                var d = (mPos - hh.Components.state.currentCell).magnitude;
                if (d < _config.maxRange)
                {
                    _enemiesInRange.Add(hh);
                }
            }
        }
        
        private SpellParticlesByLevel GetFxView()
        {
            if (_fx != null) return _fx;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Terrorize);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            _fx = instance;
            return instance;
        }
    }
    
}