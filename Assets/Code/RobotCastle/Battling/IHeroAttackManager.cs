using System;

namespace RobotCastle.Battling
{
    public interface IHeroAttackManager
    {
        public event Action OnAttackStep;
        
        IAttackAction AttackAction { get; set; }
        IAttackHitAction HitAction { get; set; }

        void SetDefaultActions();
        
        IHeroController Hero { get; set; }
        public IDamageReceiver LastTarget { get; }
        void BeginAttack(IDamageReceiver target);
        void Stop();
    }
}