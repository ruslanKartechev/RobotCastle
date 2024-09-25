namespace RobotCastle.Battling
{
    public class DamageDecoratorPlusDamage : IDamageDecorator
    {
        public DamageDecoratorPlusDamage(float val, EDamageType type)
        {
            this.val = val;
            this.type = type;
        }
        
        public float val;
        public EDamageType type;

        public string id => $"plus_damage_{val}";

        public DamageArgs Apply(DamageArgs damageArgs)
        {
            switch (type)
            {
                case EDamageType.Magical:
                    damageArgs.magicDamage += val;
                    break;
                case EDamageType.Physical:
                    damageArgs.physDamage += val;
                    break;
            }
            return damageArgs;
        }
    }
}