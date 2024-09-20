namespace RobotCastle.Battling
{
    public interface IKillProcessor
    {
        void OnKilled();
    }

    public interface IHeroRestartProcessor
    {
        void Restart();
    }
}