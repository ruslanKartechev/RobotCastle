using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using RobotCastle.Utils;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellRapidArrows : Spell, IFullManaListener, IHeroProcess, IStatDecorator, IProjectileFactory
    {
        private static Pool _projectilesPool;

        private static void CreatePoolIfNot()
        {
            if(_projectilesPool != null) 
                return;
            _projectilesPool = ServiceLocator.Get<ISimplePoolsManager>().AddPoolIfNot("bullet_hansi_fire", "prefabs/projectiles/bullet_hansi_fire", 10);
            _projectilesPool.Init();
        }
        
        public SpellRapidArrows(SpellConfigRapidArrows config, HeroComponents components)
        {
            _components = components;
            _config = config;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            CreatePoolIfNot();
        }

        public void OnFullMana(GameObject heroGo)
        {
            if(_isActive) return;
            _token = new CancellationTokenSource();
            Work(_token.Token);
        }
        
        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _fxView.gameObject.SetActive(false);
                _components.stats.AttackSpeed.RemoveDecorator(this);
                var atkManager = _components.attackManager as HeroRangedAttackManager;
                atkManager.ProjectileFactory = _prevFactory;
                _manaAdder.CanAdd = true;
                _token?.Cancel();
            }
        }
        
        public string name => "rapid_arrows";
        
        public int order => 1;
        
        public float Decorate(float val)
        {
            var lvl = (int)HeroesManager.GetSpellTier(_components.stats.MergeTier);
            return val * _config.atkSpeedMultiplier[lvl];
        }
        
        public IProjectile GetProjectile()
        {
            return _projectilesPool.GetOne() as IProjectile;
        }

        private SpellConfigRapidArrows _config;
        private CancellationTokenSource _token;
        private SpellParticlesOnHero _fxView;
        private ConditionedManaAdder _manaAdder;
        private IProjectileFactory _prevFactory;

        private async void Work(CancellationToken token)
        {
            _isActive = true;
            _manaAdder.CanAdd = false;
            _components.processes.Add(this);
            
            var atkManager = _components.attackManager as HeroRangedAttackManager;
            _prevFactory = atkManager.ProjectileFactory;
            atkManager.ProjectileFactory = this;

            _components.stats.AttackSpeed.AddDecorator(this);
            var fx = GetFxView();
            fx.ShowTrackingDefaultDuration(_components.transform);
            
            _components.heroUI.ManaUI.AnimateTimedSpell(1f, 0f, _config.duration);
            await HeroesManager.WaitGameTime(_config.duration, token);
            if (token.IsCancellationRequested)
                return;
            
            _components.stats.AttackSpeed.RemoveDecorator(this);
            fx.gameObject.SetActive(false);
            _manaAdder.CanAdd = true;
            atkManager.ProjectileFactory = _prevFactory;
            _components.stats.ManaResetAfterFull.Reset(_components);
            _components.processes.Remove(this);
            _isActive = false;
        }
        
        private SpellParticlesOnHero GetFxView()
        {
            if(_fxView == null)
            {
                var prefab = Resources.Load<GameObject>(HeroesConstants.SpellFXPrefab_RapidArrow);
                _fxView = Object.Instantiate(prefab).GetComponent<SpellParticlesOnHero>();
            }
            _fxView.transform.position = _components.transform.position;
            _fxView.transform.rotation = _components.transform.rotation;
            return _fxView;
        }

    }
}