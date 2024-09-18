using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHeroAttackManager
    {
        HeroController Hero { get; set; }
        void BeginAttack(Transform targetTransform, IDamageReceiver target);
        void Stop();
    }
}