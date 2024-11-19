using System.Threading;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellBloodLust : Spell, IFullManaListener, IHeroProcess, IStatDecorator
    {
        public SpellBloodLust(SpellConfigBloodLust config, HeroComponents components)
        {
            _config = config;
            _components = components;
            Setup(config, out _manaAdder);
        }

        public void OnFullMana(GameObject heroGo)
        {
            if (_isActive) return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            Working(_token.Token);
        }

        public void Stop()
        {
            if (!_isActive) return;
            _token?.Cancel();
            _isActive = false;
            _components.stats.AttackSpeed.RemoveDecorator(this);
        }

        public string name => "Spell SpellBloodLust";
        public int order => 2;
        public float Decorate(float val) => val * (1 + _config.speedBonus);

        private CancellationTokenSource _token;
        private SpellConfigBloodLust _config;
        private ConditionedManaAdder _manaAdder;

        private async void Working(CancellationToken token)
        {
            _manaAdder.CanAdd = false;
            _components.stats.AttackSpeed.AddDecorator(this);
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, _config.duration);
            await HeroesManager.WaitGameTime(_config.duration, token);
            if (token.IsCancellationRequested) return;
            _components.stats.AttackSpeed.RemoveDecorator(this);
            _components.stats.ManaResetAfterFull.Reset(_components);
            _manaAdder.CanAdd = true;
            _isActive = false;
        }
    }
    
    
}