using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroMeleeAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;

        public IHeroController Hero { get; set; }
        public IDamageReceiver CurrentTarget => _target;
        
        private IDamageReceiver _target;
        private bool _activated;
        
        public void BeginAttack(Transform targetTransform, IDamageReceiver target)
        {
            _target = target;
            if (!_activated)
            {
                _activated = true;
                Hero.View.animationEventReceiver.OnAttackEvent -= OnAttack;
                Hero.View.animationEventReceiver.OnAttackEvent += OnAttack;
            }
            Hero.View.animator.SetBool(HeroesConfig.Anim_Attack, true);
        }

        public void Stop()
        {
            if (!_activated)
                return;
            _activated = false;
            Hero.View.animationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.View.animator.SetBool(HeroesConfig.Anim_Attack, false);
        }

        private void OnAttack()
        {
            if (!_activated)
                return;
            var damage = Hero.View.damageSource.GetDamage();
            _target.TakeDamage(damage);
            OnAttackStep?.Invoke();
            Hero.View.stats.ManaAdder.AddMana(damage.physDamage * HeroesConfig.ManaGainDamageMultiplier);
        }
    }
}