namespace RobotCastle.Battling
{
    public class NumberedDamageModifier
    {
        public bool IsOut => number <= 0;

        public NumberedDamageModifier(IDamageCalculationModifier modifier, IHeroController hero, int number)
        {
            this.modifier = modifier;
            this.hero = hero;
            this.number = number;
        }

        public void Add()
        {
            if (_added || number == 0) return;
            _added = true;
            hero.Components.damageSource.AddModifier(modifier);
            hero.Components.attackManager.OnAttackStep += OnAttack;
        }

        public void Disable()
        {
            if (!_added) return;
            _added = false;
            hero.Components.damageSource.AddModifier(modifier);
            hero.Components.attackManager.OnAttackStep -= OnAttack;
        }
        
        public IDamageCalculationModifier modifier;
        public IHeroController hero;
        public int number;
        private bool _added;

        
        private void OnAttack()
        {
            number--;
            if (number <= 0)
            {
                Disable();
            }
        }
    }
}