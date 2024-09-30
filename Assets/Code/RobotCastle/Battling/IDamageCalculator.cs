namespace RobotCastle.Battling
{
    public interface IDamageCalculator
    {
        DamageArgs CalculateRegularAttackDamage();
        DamageArgs CalculateSpellAttackDamage();
        void AddDecorator(IDamageDecorator decorator);
        void RemoveDecorator(IDamageDecorator decorator);
        void ClearAllDecorators();
    }
}