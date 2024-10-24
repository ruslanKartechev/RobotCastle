namespace RobotCastle.Battling
{
    public interface IKIllModifier
    {
        int order { get; }
        void OnKilled(HeroComponents components);
    }
}