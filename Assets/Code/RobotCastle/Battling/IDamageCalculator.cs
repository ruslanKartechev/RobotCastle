namespace RobotCastle.Battling
{
    public interface IDamageCalculator
    {
        HeroDamageArgs CalculatePhysDamage();
        
        HeroDamageArgs CalculateSpellDamage();
        
    }
}