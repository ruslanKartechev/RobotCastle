using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroMeleeAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;

        public IHeroController Hero { get; set; }
        public IDamageReceiver CurrentTarget => _target;
        public IDamageReceiver LastTarget => _target;
        
        private IDamageReceiver _target;
        private bool _activated;

        public void BeginAttack(IDamageReceiver target)
        {
            _target = target;
            if (!_activated)
            {
                _activated = true;
                Hero.View.animationEventReceiver.OnAttackEvent -= OnAttack;
                Hero.View.animationEventReceiver.OnAttackEvent += OnAttack;
            }
            Hero.View.state.isAttacking = true;
            Hero.View.animator.SetBool(HeroesConstants.Anim_Attack, true);
        }

        public void Stop()
        {
            if (!_activated)
                return;
            _activated = false;
            Hero.View.animationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.View.animator.SetBool(HeroesConstants.Anim_Attack, false);
            Hero.View.state.isAttacking = false;
        }

        private void OnAttack()
        {
            if (!_activated)
                return;
            var damage = Hero.View.stats.DamageCalculator.CalculateRegularAttackDamage();
            _target.TakeDamage(damage);
            OnAttackStep?.Invoke();
            Hero.View.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
        }
    }
}