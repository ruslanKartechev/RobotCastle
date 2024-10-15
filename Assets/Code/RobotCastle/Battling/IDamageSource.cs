namespace RobotCastle.Battling
{
    public interface IDamageSource : IDamageCalculator
    {
        void DamagePhys(IDamageReceiver receiver);
        void DamageSpellAndPhys(IDamageReceiver receiver);
        void ReflectDamageBack();
    }
}