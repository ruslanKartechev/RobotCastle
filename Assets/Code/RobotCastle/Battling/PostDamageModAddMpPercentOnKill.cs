using SleepDev;

namespace RobotCastle.Battling
{
    public class PostDamageModAddMpPercentOnKill : IPostDamageModifier
    {
        public PostDamageModAddMpPercentOnKill(HeroComponents components, float addedMpPercent)
        {
            _components = components;
            _addedMpPercent = addedMpPercent;
        }

        public void Apply(DamageReceivedArgs receivedArgs)
        {
            if (receivedArgs.diedAfter)
            {
                CLog.LogRed("=========== died after damage, adding mp");
                _components.stats.ManaAdder.AddMana(_addedMpPercent * _components.stats.ManaMax.Get());
            }
        }

        protected HeroComponents _components;
        protected float _addedMpPercent;
    }

    
    
    public class RecurringPostDamageModAddMpPercentOnKill : PostDamageModAddMpPercentOnKill, IRecurringModificator
    {
        public RecurringPostDamageModAddMpPercentOnKill(HeroComponents components, float addedMpPercent) :
            base(components, addedMpPercent)
        {
        }

        public void Activate()
        {
            _components.damageSource.AddModifier(this);
        }

        public void Deactivate()
        {
            _components.damageSource.RemoveModifier(this);
        }
    }
}