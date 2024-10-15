namespace RobotCastle.Battling
{
    public interface IDamageTakenModifiers
    {
        int priority { get; }
        
        HeroDamageArgs Apply(HeroDamageArgs damageInput);
        
    }
}