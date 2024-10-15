namespace RobotCastle.Battling
{
    public interface IDamageReceiver : IGameObjectComponent
    {
        /// <returns>Amount of damage actually received after all defence stats</returns>
        DamageArgs TakeDamage(DamageArgs args);
    }
}