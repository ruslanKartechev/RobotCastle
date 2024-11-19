namespace RobotCastle.Battling
{
    public abstract class Spell
    {
        protected HeroComponents _components;
        protected bool _isActive;

        protected void Setup(BaseSpellConfig config, out ConditionedManaAdder manaAdder)
        {
            _components.stats.ManaMax.SetBaseAndCurrent(config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(config.manaStart); 
            _components.stats.ManaAdder = manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(config.manaMax, config.manaStart);
            _components.stats.ManaGainPerAttack = config.manaGain;
        }
    }
}