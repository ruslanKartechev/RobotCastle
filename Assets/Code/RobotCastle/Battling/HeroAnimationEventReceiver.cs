using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroAnimationEventReceiver : MonoBehaviour
    {
        public event Action OnAttackEvent;
        public event Action OnAttackEvent2;
        
        public event Action OnAttackAnimationEndEvent;
        

        /// <summary>
        /// Called from Unity Animator
        /// </summary>
        public void AE_Attack()
        {
            OnAttackEvent?.Invoke();
        }
        
        public void AE_Attack_2()
        {
            OnAttackEvent2?.Invoke();
        }

        public void AE_AttackEnd()
        {
            OnAttackAnimationEndEvent?.Invoke();
        }

    }
}