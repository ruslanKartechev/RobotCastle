using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellBombard : Spell, IHeroProcess, IFullManaListener
    {
        public SpellBombard(SpellConfigBombard config, HeroComponents components)
        {
            _components = components;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
        }
        
        public void Stop()
        {
            _token?.Cancel();
        }

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.StopCurrentBehaviour();
            Working(_token.Token);
        }
        
        private SpellConfigBombard _config;
        private CancellationTokenSource _token;
        private SpellParticlesByLevel _fxView;
        private ConditionedManaAdder _manaAdder;
        private bool _isCasting;

        private async void Working(CancellationToken token)
        {
            _manaAdder.CanAdd = false;
            while (!token.IsCancellationRequested)
            {
                _components.stats.ManaResetAfterFull.Reset(_components);
                var elapsed = 0f;
                var time = _config.loadDuration;
                _components.heroUI.ManaUI.AnimateTimedSpell(0f, 1f, time);
                while (!token.IsCancellationRequested && elapsed <= time)
                {

                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }

                if (token.IsCancellationRequested) return;
                _components.animator.Play("Cast", 0, 0);
                _components.animationEventReceiver.OnAttackEvent += OnAttack;
                _isCasting = true;
                while (!token.IsCancellationRequested && _isCasting)
                    await Task.Yield();
                if (token.IsCancellationRequested) return;
                var fx = GetFxView();
                fx.PlayLevelAtPoint(_components.transform.position, 0);
                Spawn();
                
                if (token.IsCancellationRequested) return;
            }
        }

        private void OnAttack()
        {
            _components.animationEventReceiver.OnAttackEvent -= OnAttack;
            _isCasting = false;   
        }

        private void Spawn()
        {
            var count = _config.spawnCount.Val;
            var args = new List<SpawnArgs>(count);
            for (var i = 0; i < count; i++)
            {
                var arg = new SpawnArgs(_config.spawnUnit);
                arg.usePreferredCoordinate = false;
                args.Add(arg);
            }
            BattleManager.SetClosestAvailableDesiredPositions(args, _components.state.currentCell);
            ServiceLocator.Get<BattleManager>().AddNewEnemiesDuringBattle(args);
        }
        
        private SpellParticlesByLevel GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_Bombard);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesByLevel>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }

    }
}