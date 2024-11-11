namespace RobotCastle.Battling
{
    public class DamageModifierChangeTypeAndAmount : IDamageCalculationModifier
    {
        public int amount;
        public EDamageType type;
        
        public int order { get; set; } = 100;
        
        public HeroDamageArgs Apply(HeroDamageArgs damageArgs)
        {
            if (damageArgs.reflected)
                return damageArgs;
            damageArgs.type = type;
            damageArgs.amount = amount;
            return damageArgs;
        }
    }
}