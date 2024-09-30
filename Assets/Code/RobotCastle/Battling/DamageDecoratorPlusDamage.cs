using SleepDev;

namespace RobotCastle.Battling
{
    public class DamageDecoratorPlusDamage : IDamageDecorator
    {
        public DamageDecoratorPlusDamage(float addedPhysDamage, float addedMagicDamage)
        {
            this.addedPhysDamage = addedPhysDamage;
            this.addedMagicDamage = addedMagicDamage;
        }

        public float addedPhysDamage;
        public float addedMagicDamage;

        public string id => $"plus_damage";

        public DamageArgs Apply(DamageArgs damageArgs)
        {
            CLog.LogRed($"************ Added damage: {addedPhysDamage}, {addedMagicDamage}");
            damageArgs.magicDamage += addedMagicDamage;
            damageArgs.physDamage += addedPhysDamage;
            return damageArgs;
        }
    }
}