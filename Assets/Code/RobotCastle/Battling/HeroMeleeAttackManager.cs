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

        public IAttackAction AttackAction
        {
            get => _attackAction;
            set => _attackAction = value;
        }        
        
        public IAttackHitAction HitAction
        {
            get => _hitAction;
            set => _hitAction = value;
        }       
        
        public void BeginAttack(IDamageReceiver target)
        {
            if (HitAction == null)
                HitAction = new MeleeHitAction(Hero.Components);
            if (AttackAction == null)
                AttackAction = new MeleeAttackAction(Hero.Components);
            
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

        public void SetDefaultActions()
        {
            HitAction = new MeleeHitAction(Hero.Components);
            AttackAction = new MeleeAttackAction(Hero.Components);
        }

        private IDamageReceiver _target;
        private bool _activated;
        private IAttackAction _attackAction;
        private IAttackHitAction _hitAction;

        private void OnAttack()
        {
            if (!_activated)
                return;
            AttackAction.Attack(_target, 0);
            HitAction.Hit(_target);
            OnAttackStep?.Invoke();
        }

    }
}