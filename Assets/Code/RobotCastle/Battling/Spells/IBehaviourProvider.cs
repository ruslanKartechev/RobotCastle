namespace RobotCastle.Battling
{
    public interface IBehaviourProvider
    {
        IHeroBehaviour GetBehaviour(IHeroController hero);
    }
}