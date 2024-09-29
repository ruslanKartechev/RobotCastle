using System.Threading;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellIronWill : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public float BaseSpellPower => _config.spellResist[(int)HeroesHelper.GetSpellTier(_view.stats.MergeTier)];
        
        public string name => "spell";
        public int priority => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellIronWill(HeroView view, SpellConfigIronWill config)
        {
            _view = view;
            _config = config;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_view);
            _view.stats.SpellPower.PermanentDecorators.Add(this);
        }
        
        public void Stop()
        {
            _isActive = false;
        }
        
        public void OnFullMana(GameObject heroGo)
        {
            CLog.LogGreen($"[{_view.gameObject.name}] [{nameof(SpellIronWill)}] Adding shield bonus");
            _view.processes.Add(this);
            _view.stats.ManaCurrent.Val = 0;
            _view.stats.Shield = _view.stats.SpellPower.Get();
            _view.heroUI.ShieldBar.TrackUntilZero(_view.stats);
        }
        
        private SpellConfigIronWill _config;
        private SpellParticleOnGridEffect _fxView;
        private ConditionedManaAdder _manaAdder;
        private CancellationTokenSource _token;

        
        private void OnAfterDamage()
        {
            if (!_isActive) return;
            CLog.LogGreen($"[{nameof(SpellIronWill)}] Reverting defence bonus");
            _view.healthManager.OnAfterDamage -= OnAfterDamage;
            _view.stats.PhysicalResist.SetBase();
            _view.stats.MagicalResist.SetBase();
            _isActive = false;
            _view.processes.Remove(this);
        }
        
    }
}