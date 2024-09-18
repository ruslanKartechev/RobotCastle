using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroAnimationEventReceiver : MonoBehaviour
    {
        public event Action OnAttackEvent;

        /// <summary>
        /// Called from Unity Animator
        /// </summary>
        public void AE_Attack()
        {
            OnAttackEvent?.Invoke();
        }
        

    }
}