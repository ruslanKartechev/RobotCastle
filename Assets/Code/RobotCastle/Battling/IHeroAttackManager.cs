using System;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHeroAttackManager
    {
        public event Action OnAttackStep;
        HeroController Hero { get; set; }
        void BeginAttack(Transform targetTransform, IDamageReceiver target);
        void Stop();
    }
}