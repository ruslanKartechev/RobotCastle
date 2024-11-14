namespace RobotCastle.Battling
{
    public class DefaultBehaviourProvider : IBehaviourProvider
    {
        public IHeroBehaviour GetBehaviour(IHeroController hero)
        {
            return new HeroAttackEnemyBehaviour();
        }
    }
}