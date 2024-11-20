namespace RobotCastle.Battling
{
    public class MeleeHitAction : IAttackHitAction
    {
        public MeleeHitAction(HeroComponents components)
        {
            _components = components;
        }
        
        public void Hit(object target)
        {
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                _components.damageSource.DamagePhys(dm);
                _components.stats.ManaAdder.AddDefault();
            }
        }
        
        private HeroComponents _components;
    }
}