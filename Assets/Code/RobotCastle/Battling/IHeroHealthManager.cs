using RobotCastle.Core;

namespace RobotCastle.Battling
{
    public interface IHeroHealthManager : IModifiable<IDamageTakenModifiers>
    {
        void SetDamageable(bool damageable);
  
    }
}