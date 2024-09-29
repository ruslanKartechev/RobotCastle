using System;

namespace RobotCastle.Battling
{
    public interface IHeroHealthManager
    {
        event Action OnAfterDamage;
        event Action OnDamagePreApplied;
        event Action OnDamageAttempted;
        
        
        void SetDamageable(bool damageable);

    }
}