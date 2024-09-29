namespace RobotCastle.Battling
{
    public interface IDamageReceiver : IGameObjectComponent
    {
        void TakeDamage(DamageArgs args);
    }
}