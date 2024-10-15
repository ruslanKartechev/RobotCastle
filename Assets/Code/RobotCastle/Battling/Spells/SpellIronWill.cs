using System.Threading;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellIronWill : Spell, IFullManaListener, IStatDecorator, IHeroProcess
    {
        public float BaseSpellPower => _config.spellResist[(int)HeroesManager.GetSpellTier(_view.stats.MergeTier)];
        
        public string name => "spell";
        public int order => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }
        
        public SpellIronWill(HeroComponents view, SpellConfigIronWill config)
        {
            _view = view;
            _config = config;
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_view);
            _view.stats.SpellPower.AddPermanentDecorator(this);
            _modShield = new DamageTakeModShield(0f, view);
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
            _modShield.AddToHero(_view.stats.SpellPower.Get());
            _view.heroUI.ShieldBar.TrackUntilZero(_modShield);
            var fx = GetFxView();
            fx.Show(_view.transform);
        }
        
        private SpellConfigIronWill _config;
        private CancellationTokenSource _token;
        private SpellParticlesOnHero _fxView;
        private DamageTakeModShield _modShield;
        private ConditionedManaAdder _manaAdder;


        private SpellParticlesOnHero GetFxView()
        {
            if (_fxView != null) return _fxView;
            var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_IronWill);
            var instance = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            _fxView = instance;
            return instance;
        }

        
    }
}