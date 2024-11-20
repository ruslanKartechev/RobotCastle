using SleepDev;

namespace RobotCastle.Battling
{
    public class MeleeAttackAction : IAttackAction
    {
        public MeleeAttackAction(HeroComponents components)
        {
            _components = components;
        }
        
        public void Attack(IDamageReceiver target, int animationIndex)
        {
            if (_components.attackSounds.Count > 0)
            {
                var s = _components.attackSounds.Random();
                s.Play();
            }
        }

        private HeroComponents _components;
    }
}