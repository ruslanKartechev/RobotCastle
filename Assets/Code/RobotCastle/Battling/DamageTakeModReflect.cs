using SleepDev;

namespace RobotCastle.Battling
{
    /// <summary>
    /// Is applied after shield. Does not reflect damage "eaten" by shield or mighty block
    /// </summary>
    public class DamageTakeModReflect : IDamageTakenModifiers, IRecurringModificator
    {
        public DamageTakeModReflect(HeroComponents components, float percentage)
        {
            _components = components;
            _percentReflected = percentage;
        }
        
        public int priority { get; } = 10;

        public HeroDamageArgs Apply(HeroDamageArgs damageInput)
        {
            if (damageInput.reflected == false)
            {
                var amount = damageInput.amount * _percentReflected * (damageInput.type == EDamageType.Magical ? .5f : 1f);
                CLog.LogWhite($"Reflected: {amount} damage back");
                var reflectedDamage = new HeroDamageArgs(amount, EDamageType.Physical, _components, true, false);
                damageInput.source.damageReceiver.TakeDamage(reflectedDamage);
            }
            return damageInput;
        }

        public void Activate()
        {
            _components.healthManager.AddModifier(this);
        }

        public void Deactivate()
        {
            _components.healthManager.RemoveModifier(this);
        }

        private HeroComponents _components;
        private float _percentReflected;
    }
}