namespace RobotCastle.Battling
{
    public interface IDamageCalculator
    {
        DamageArgs CalculatePhysDamage();
        
        DamageArgs CalculateSpellAndPhysDamage();
        
        void AddDecorator(IDamageDecorator decorator);
        void RemoveDecorator(IDamageDecorator decorator);
        void ClearAllDecorators();
    }
}