namespace RobotCastle.Battling
{
    public interface IDamageSource : IDamageCalculator
    {
        void Damage(IDamageReceiver receiver, HeroDamageArgs args);
        void DamagePhys(IDamageReceiver receiver);
        void DamageSpellAndPhys(IDamageReceiver receiver);
        void DamageSpell(IDamageReceiver receiver);
        void DamageSpellAndPhys(float phys, float mag, IDamageReceiver receiver);

        void AddModifier(IDamageCalculationModifier mod);
        void RemoveModifier(IDamageCalculationModifier mod);
        
        void AddModifier(IPostDamageModifier mod);
        void RemoveModifier(IPostDamageModifier mod);
        
        void ClearPostDamageModifiers();
        void ClearDamageCalculationModifiers();
        
        IBattleDamageStatsCollector StatsCollector { get; set; }
    }
}