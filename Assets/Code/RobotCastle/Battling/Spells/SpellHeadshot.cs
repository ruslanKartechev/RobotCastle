using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellHeadshot : IFullManaListener, ISpellPowerGetter
    {
        private SpellConfigHeadshot _config;
        private HeroView _view;
        private DamageDecoratorPlusDamage _decorator;
        private DamageDecoratorPlusDamage _decoratorPhys;
        private bool _isActive;
        // ADD IGNORE MANA ADD DECORATOR FOR DAMAGE !

        public SpellHeadshot(HeroView heroView, SpellConfigHeadshot config)
        {
            _config = config;
            _view = heroView;
            _decorator = new DamageDecoratorPlusDamage(0, EDamageType.Magical);
            _decoratorPhys = new DamageDecoratorPlusDamage(0, EDamageType.Physical);
        }

        public float BaseSpellPower => _config.spellDamage[_view.Stats.MergeTier];

        /// <summary>
        /// </summary>
        /// <returns>Current spell power</returns>
        public float FullSpellPower
        {
            get
            {
                var v = BaseSpellPower;
                foreach (var dec in _view.Stats.SpellPowerDecorators)
                    v = dec.Decorate(v);
                return v;
            }
        }

        public void OnFullMana(GameObject heroGo)
        {
            Execute();
        }

        private void Execute()
        {
            if (_isActive)
                return;
            _isActive = true;
            CLog.LogGreen($"[{_view.gameObject.name}] Spell headshot executed");
            _view.DamageSource.AddDecorator(_decorator);
            _view.DamageSource.AddDecorator(_decoratorPhys);
            _view.AttackManager.OnAttackStep -= OnAttack;
            _view.AttackManager.OnAttackStep += OnAttack;
        }

        private void OnAttack()
        {
            _view.DamageSource.RemoveDecorator(_decorator);
            _view.DamageSource.RemoveDecorator(_decoratorPhys);
            _view.AttackManager.OnAttackStep -= OnAttack;
            _view.Stats.ManaReset.Reset(_view);
            _isActive = false;
        }
    }
}