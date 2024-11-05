namespace RobotCastle.Battling
{
    public interface IBattleHeroKilledListener
    {
        void OnKilled(IHeroController hero);
    }
}