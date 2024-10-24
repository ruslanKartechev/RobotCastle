using System.Threading;
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
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.SpellPower.AddPermanentDecorator(this);
            _modShield = new DamageTakeModShield(0f, view);
        }
        
        public void Stop()
        {
            _isActive = false;
            if (_isCasting)
            {
                _components.animationEventReceiver.OnAttackEvent -= OnAttack;
                _isCasting = false;
            }
        }

        public void OnFullMana(GameObject heroGo)
        {
            CLog.LogGreen($"[{_components.gameObject.name}] [{nameof(SpellIronWill)}] Adding shield bonus");
            _components.processes.Add(this);
            _components.stats.ManaCurrent.Val = 0;
            _manaAdder.CanAdd = false;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            CastAnimated(_token.Token);
        }

        private void AddShield()
        {
            _modShield.AddToHero(_components.stats.SpellPower.Get());
            _components.heroUI.ShieldBar.TrackUntilZero(_modShield);
            var fx = GetFxView();
            fx.Show(_components.transform);
        }

        private async void CastAnimated(CancellationToken token)
        {
            const float afterCastDelay = .4f;
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.PauseCurrentBehaviour();
            
            _isCasting = true;
            _components.animator.Play("Cast");
            _components.animationEventReceiver.OnAttackEvent += OnAttack;
            
            while (_isCasting && !token.IsCancellationRequested)
                await Task.Yield();
            if (token.IsCancellationRequested) return;
            _isCasting = false;
            AddShield();
            
            await Task.Delay((int)(1000 * afterCastDelay), token);
            if (token.IsCancellationRequested) return;

            hero.ResumeCurrentBehaviour();
            _manaAdder.CanAdd = true;
            _isActive = false;
            
        }
        
        private SpellConfigIronWill _config;
        private CancellationTokenSource _token;
        private SpellParticlesOnHero _fxView;
        private DamageTakeModShield _modShield;
        private ConditionedManaAdder _manaAdder;
        private bool _isCasting;

        private void OnAttack()
        {
            _isCasting = false;
        }

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