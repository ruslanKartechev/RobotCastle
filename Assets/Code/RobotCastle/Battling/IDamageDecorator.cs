namespace RobotCastle.Battling
{

    public interface IDamageCalculationModifier
    {
        int order { get; }
        HeroDamageArgs Apply(HeroDamageArgs damageArgs);
    }

    public interface IPostDamageModifier
    {
        void Apply(DamageReceivedArgs receivedArgs);
    }
}