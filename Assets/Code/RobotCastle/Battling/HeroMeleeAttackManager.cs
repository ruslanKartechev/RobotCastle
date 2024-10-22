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
                Hero.Components.animationEventReceiver.OnAttackEvent -= OnAttack;
                Hero.Components.animationEventReceiver.OnAttackEvent += OnAttack;
                if (_doOverrideAnimations)
                {
                    Hero.Components.animationEventReceiver.OnAttackAnimationEndEvent -= SetRandomAnimation;
                    Hero.Components.animationEventReceiver.OnAttackAnimationEndEvent += SetRandomAnimation;
                }
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
            if (_doOverrideAnimations)
                Hero.Components.animationEventReceiver.OnAttackAnimationEndEvent -= SetRandomAnimation;
        }

        private void OnAttack()
        {
            if (!_activated)
                return;
            Hero.Components.damageSource.DamagePhys(_target);
            OnAttackStep?.Invoke();
            Hero.Components.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
        }

        private void SetRandomAnimation()
        {
            Hero.Components.animator.runtimeAnimatorController = _overrideControllers.Random();
        }
    }
}