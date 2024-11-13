using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellRainOfBombs : Spell, IFullManaListener, IHeroProcess
    {
        public SpellRainOfBombs(SpellConfigRainOfBombs config, HeroComponents components)
        {
            _config = config;
            _components = components;
            _components.processes.Add(this);
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _isWaitingForAttack = false;
                _components.attackManager.OnAttackStep -= OnAttack;
                _components.stats.AttackSpeed.RemoveDecorator(_multiplier);
                _components.attackManager.Stop();
                _token?.Cancel();
            }
        }

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive)
                return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _isActive = true;
            Working(_token.Token);
        }
        
        private CancellationTokenSource _token;
        private SpellConfigRainOfBombs _config;
        private ConditionedManaAdder _manaAdder;
        private SimpleDecoratorMultiplier _multiplier;
        private bool _isWaitingForAttack;
        
        private async void Working(CancellationToken token)
        {
            _manaAdder.CanAdd = false;
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            var duration = _config.duration[lvl] + _components.stats.SpellPower.Get() / 50f;
            var atks = _config.attackSpeed[lvl];
            if (_multiplier == null)
                _multiplier = new SimpleDecoratorMultiplier(atks, 5, "rain_of_bombs");
            else
                _multiplier.multiplier = atks;
                
            _components.stats.AttackSpeed.AddDecorator(_multiplier);
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            
            await Task.Yield();
            if (token.IsCancellationRequested) return;

            var allEnemies = hero.Battle.GetTeam(hero.TeamNum).enemyUnits;
            var elapsed = 0f;
            _components.attackManager.OnAttackStep += OnAttack;
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, duration);
            while (elapsed < duration && allEnemies.Count > 0 && !token.IsCancellationRequested)
            {
                var r = allEnemies.Random();
                _components.attackManager.BeginAttack(r.Components.damageReceiver);
                _isWaitingForAttack = true;
                while (_isWaitingForAttack && allEnemies.Count > 0 && !token.IsCancellationRequested)
                {
                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            if (token.IsCancellationRequested) return;
            _components.attackManager.Stop();
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.stats.AttackSpeed.RemoveDecorator(_multiplier);
            _components.processes.Remove(this);
            hero.ResumeCurrentBehaviour();
            _manaAdder.CanAdd = true;
            _isActive = false;
        }

        private void OnAttack()
        {
            _isWaitingForAttack = false;
        }
        
        
    }
}