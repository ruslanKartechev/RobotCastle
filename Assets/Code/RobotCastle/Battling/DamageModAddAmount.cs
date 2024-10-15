using SleepDev;

namespace RobotCastle.Battling
{
    public class DamageModAddAmount : IDamageCalculationModifier
    {
        public DamageModAddAmount(float addedPhysDamage, float addedMagicDamage)
        {
            this.addedPhysDamage = addedPhysDamage;
            this.addedMagicDamage = addedMagicDamage;
        }

        public float addedPhysDamage;
        public float addedMagicDamage;

        public string id => $"plus_damage";

        public int order => 0;

        public HeroDamageArgs Apply(HeroDamageArgs damageArgs)
        {
            var added = 0f;
            switch (damageArgs.type)
            {
                case EDamageType.Magical:
                    added += addedMagicDamage;
                    break;
                case EDamageType.Physical:
                    added += addedPhysDamage;
                    break;
            }
            damageArgs.amount += added;
            CLog.Log($"[DamageDecoratorPlusDamage] Added {added}");
            return damageArgs;
        }
    }
}