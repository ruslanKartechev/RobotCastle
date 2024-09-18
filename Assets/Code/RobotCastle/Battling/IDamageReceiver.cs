namespace RobotCastle.Battling
{
    public interface IDamageReceiver
    {
        bool IsDamageable { get; }
        void TakeDamage(DamageArgs args);
    }
}