namespace RobotCastle.Battling
{
    public interface IDamageReceiver : IGameObjectComponent
    {
        DamageReceivedArgs TakeDamage(HeroDamageArgs args);
        
    }
}