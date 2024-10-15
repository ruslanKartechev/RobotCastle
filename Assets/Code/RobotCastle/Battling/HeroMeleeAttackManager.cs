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

        [SerializeField] private bool _doOverrideAnimations;
        [SerializeField] private List<RuntimeAnimatorController> _overrideControllers;
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
                if (_doOverrideAnimations)
                {
                    Hero.View.animationEventReceiver.OnAttackAnimationEndEvent -= SetRandomAnimation;
                    Hero.View.animationEventReceiver.OnAttackAnimationEndEvent += SetRandomAnimation;
                }
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
            if (_doOverrideAnimations)
                Hero.View.animationEventReceiver.OnAttackAnimationEndEvent -= SetRandomAnimation;
        }

        private void OnAttack()
        {
            if (!_activated)
                return;
            Hero.View.damageSource.DamagePhys(_target);
            OnAttackStep?.Invoke();
            Hero.View.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
        }

        private void SetRandomAnimation()
        {
            Hero.View.animator.runtimeAnimatorController = _overrideControllers.Random();
        }
    }
}