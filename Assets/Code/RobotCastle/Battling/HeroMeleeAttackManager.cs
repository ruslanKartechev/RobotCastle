using System;
using System.Collections.Generic;
using SleepDev;
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
                Hero.Components.animationEventReceiver.OnAttackEvent -= OnAttack;
                Hero.Components.animationEventReceiver.OnAttackEvent += OnAttack;
        
            }
            Hero.Components.state.isAttacking = true;
            Hero.Components.animator.SetBool(HeroesConstants.Anim_Attack, true);
        }

        public void Stop()
        {
            if (!_activated)
                return;
            _activated = false;
            Hero.Components.animationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.Components.animator.SetBool(HeroesConstants.Anim_Attack, false);
            Hero.Components.state.isAttacking = false;
        }

        private void OnAttack()
        {
            if (!_activated)
                return;
            Hero.Components.damageSource.DamagePhys(_target);
            OnAttackStep?.Invoke();
            Hero.Components.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
        }

      
    }
}