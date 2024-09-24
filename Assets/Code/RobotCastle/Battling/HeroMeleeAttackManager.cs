using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroMeleeAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;

        private IDamageReceiver _target;
        
        public HeroController Hero { get; set; }
        public IDamageReceiver CurrentTarget => _target;
     
        
        public void BeginAttack(Transform targetTransform, IDamageReceiver target)
        {
            _target = target;
            Hero.View.AnimationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.View.AnimationEventReceiver.OnAttackEvent += OnAttack;
            Hero.View.animator.SetBool(HeroesConfig.Anim_Attack, true);
            
        }

        public void Stop()
        {
            Hero.View.AnimationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.View.animator.SetBool(HeroesConfig.Anim_Attack, false);
        }

        private void OnAttack()
        {
            var amount = Hero.View.Stats.Attack.Val;
            var damageArgs = new DamageArgs(amount, EDamageType.Physical);
            _target.TakeDamage(damageArgs);
            OnAttackStep?.Invoke();
            
            Hero.View.Stats.AddMana(amount * HeroesConfig.ManaGainDamageMultiplier);
        }
    }
}