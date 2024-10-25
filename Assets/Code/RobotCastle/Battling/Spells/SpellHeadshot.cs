using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellHeadshot : IFullManaListener, IStatDecorator, IProjectileFactory
    {
        public SpellHeadshot(HeroComponents heroView, SpellConfigHeadshot config)
        {
            _config = config;
            _view = heroView;
            _damageMod = new DamageModAddAmount(0, 0);
            _manaAdder = new ConditionedManaAdder(_view);
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.SpellPower.AddPermanentDecorator(this);
        }
        
        public float BaseSpellPower => _config.spellDamage[(int)HeroesManager.GetSpellTier(_view.stats.MergeTier)];
        public string name => "spell";
        public int order => 10;
        public float Decorate(float val) => val + BaseSpellPower;
        
        public void OnFullMana(GameObject heroGo)
        {
            CLog.Log($"[{_view.gameObject.name}] [{nameof(SpellHeadshot)}]");
            Execute();
        }

        public IProjectile GetProjectile()
        {
            var prefab = Resources.Load<GameObject>("prefabs/projectiles/bullet_headshot_spell");
            var instanceGo = Object.Instantiate(prefab);
            instanceGo.SetActive(false);
            return instanceGo.GetComponent<IProjectile>();
        }
        
        private SpellConfigHeadshot _config;
        private HeroComponents _view;
        private DamageModAddAmount _damageMod;
        private ConditionedManaAdder _manaAdder;
        private bool _isActive;
        private IProjectileFactory _prevFactory;

        private void Execute()
        {
            if (_isActive)
                return;
            _isActive = true;
            CLog.LogWhite($"[{_view.gameObject.name}] Spell headshot executed");
            var atk = ((HeroRangedAttackManager)_view.attackManager);
            if (atk == null)
                throw new System.Exception($"{nameof(SpellHeadshot)}  cannot cast AttackManager to HeroRangedAttackManager");
            atk.OnHit -= OnHit;
            atk.OnHit += OnHit;
            _manaAdder.CanAdd = false;
            _damageMod.addedMagicDamage = _view.stats.SpellPower.Get();
            _damageMod.addedPhysDamage = _config.physDamage[(int)HeroesManager.GetSpellTier(_view.stats.MergeTier)];
            _view.damageSource.AddModifier(_damageMod);
            _prevFactory = atk.ProjectileFactory;
            atk.ProjectileFactory = this;
        }

        private void OnHit()
        {
            var atk = ((HeroRangedAttackManager)_view.attackManager);
            atk.OnHit -= OnHit;
            _view.damageSource.RemoveModifier(_damageMod);
            _view.stats.ManaResetAfterFull.Reset(_view);
            _isActive = false;
            _manaAdder.CanAdd = true;
            atk.ProjectileFactory = _prevFactory;
        }

   
    }
}