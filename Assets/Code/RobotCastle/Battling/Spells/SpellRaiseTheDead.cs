using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellRaiseTheDead : Spell, IHeroProcess, IFullManaListener
    {

        public SpellRaiseTheDead(SpellConfigRaiseTheDead config, HeroComponents components)
        {
            _components = components;
            _config = config;
            Setup(config, out _manaAdder);
        }
        
        public void Stop() => _token?.Cancel();

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _isActive = true;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _components.processes.Add(this);
 
            Working(_token.Token);
        }
        
        private SpellConfigRaiseTheDead _config;
        private CancellationTokenSource _token;
        private SpellParticlesByLevel _fxView;
        private ConditionedManaAdder _manaAdder;
        private bool _isCasting;

        private async void Working(CancellationToken token)
        {
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            _manaAdder.CanAdd = false;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, .3f);
           

            if (token.IsCancellationRequested) return;
            _components.animator.Play("Cast", 0, 0);
            _components.animationEventReceiver.OnAttackEvent += OnAttack;
            _isCasting = true;
            while (!token.IsCancellationRequested && _isCasting)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            Spawn();
            
            await HeroesManager.WaitGameTime(.3f, token);
            if (token.IsCancellationRequested) return;
            
            _manaAdder.CanAdd = true;
            _isActive = false;
            hero.ResumeCurrentBehaviour();
            _components.processes.Remove(this);
        }

        private void OnAttack()
        {
            _isCasting = false;   
        }

        private void Spawn()
        {
            var args = new List<SpawnArgs>(_config.spawnedUnits.Count);
            var lvl = _components.stats.MergeTier;
            for (var i = 0; i < _config.spawnedUnits.Count; i++)
            {
                args.Add(new SpawnArgs(_config.spawnedUnits[i]));
                args[i].coreData.level = lvl;
            }
            BattleManager.SetClosestAvailableDesiredPositions(args, _components.state.currentCell);
            ServiceLocator.Get<BattleManager>().AddNewEnemiesDuringBattle(args);
        }
        
    }
}