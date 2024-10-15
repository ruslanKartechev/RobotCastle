using SleepDev;

namespace RobotCastle.Battling
{
    public class DamageTakeModShield : IDamageTakenModifiers, IFloatGetter
    {

        public DamageTakeModShield(float shield, HeroComponents hero)
        {
            this._shield = shield;
            _hero = hero;
        }

        public int priority { get; } = 10;

        public void AddToHero(float shield)
        {
            this._shield = shield;
            _hero.healthManager.AddModifier(this);
        }
        
        public float Get() => _shield;

        
        public HeroDamageArgs Apply(HeroDamageArgs damageInput)
        {
            if (_shield <= 0)
                return damageInput;
            var damage = damageInput.amount;
            if (damage > _shield)
            {
                _shield = 0;
                damage -= _shield;
            }
            else // shield eats all damage
            {
                _shield -= damage;
                damage = 0;
            }
            damageInput.amount = damage;
            if (_shield <= 0)
                _hero.healthManager.RemoveModifier(this);
            return damageInput;
        }

        private float _shield;
        private HeroComponents _hero;

    }
}