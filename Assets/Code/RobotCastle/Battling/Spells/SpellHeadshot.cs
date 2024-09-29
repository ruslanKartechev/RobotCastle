using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellHeadshot : IFullManaListener, IStatDecorator, IProjectileFactory
    {
        public float BaseSpellPower => _config.spellDamage[(int)HeroesHelper.GetSpellTier(_view.stats.MergeTier)];
        public string name => "spell";
        public int priority => 10;
        public float Decorate(float val)
        {
            return val + BaseSpellPower;
        }

        
        public SpellHeadshot(HeroView heroView, SpellConfigHeadshot config)
        {
            _config = config;
            _view = heroView;
            _decorator = new DamageDecoratorPlusDamage(0, EDamageType.Magical);
            _decoratorPhys = new DamageDecoratorPlusDamage(0, EDamageType.Physical);
            _manaAdder = new ConditionedManaAdder(_view);
            _view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            _view.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
            _view.stats.SpellPower.PermanentDecorators.Add(this);
        }
        
        private SpellConfigHeadshot _config;
        private HeroView _view;
        private DamageDecoratorPlusDamage _decorator;
        private DamageDecoratorPlusDamage _decoratorPhys;
        private ConditionedManaAdder _manaAdder;
        private bool _isActive;
        private IProjectileFactory _prevFactory;

        public void OnFullMana(GameObject heroGo)
        {
            CLog.Log($"[{_view.gameObject.name}] [{nameof(SpellHeadshot)}]");
            Execute();
        }

        private void Execute()
        {
            if (_isActive)
                return;
            _isActive = true;
            CLog.LogWhite($"[{_view.gameObject.name}] Spell headshot executed");
            _manaAdder.CanAdd = false;
            _decorator.val = _view.stats.SpellPower.Get();
            _decoratorPhys.val = _config.physDamage[(int)HeroesHelper.GetSpellTier(_view.stats.MergeTier)];
            _view.stats.DamageCalculator.AddDecorator(_decorator);
            _view.stats.DamageCalculator.AddDecorator(_decoratorPhys);
            _view.attackManager.OnAttackStep -= OnAttack;
            _view.attackManager.OnAttackStep += OnAttack;
            var atk = ((HeroRangedAttackManager)_view.attackManager);
            _prevFactory = atk.ProjectileFactory;
            atk.ProjectileFactory = this;
        }

        private void OnAttack()
        {
            _view.stats.DamageCalculator.RemoveDecorator(_decorator);
            _view.stats.DamageCalculator.RemoveDecorator(_decoratorPhys);
            _view.attackManager.OnAttackStep -= OnAttack;
            _view.stats.ManaResetAfterFull.Reset(_view);
            _isActive = false;
            _manaAdder.CanAdd = true;
            var atk = ((HeroRangedAttackManager)_view.attackManager);
            atk.ProjectileFactory = _prevFactory;
        }

        public IProjectile GetProjectile()
        {
            var prefab = Resources.Load<GameObject>("prefabs/projectiles/bullet_headshot_spell");
            var instanceGo = Object.Instantiate(prefab);
            instanceGo.SetActive(false);
            return instanceGo.GetComponent<IProjectile>();
        }
    }
}