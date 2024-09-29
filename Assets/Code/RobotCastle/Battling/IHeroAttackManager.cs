using System;

namespace RobotCastle.Battling
{
    public interface IHeroAttackManager
    {
        public event Action OnAttackStep;
        IHeroController Hero { get; set; }
        public IDamageReceiver LastTarget { get; }
        void BeginAttack(IDamageReceiver target);
        void Stop();
    }
}