namespace RobotCastle.Battling
{
    public class SimpleManaAdder : IManaAdder
    {
        private HeroComponents _components;
        private HeroStatsManager _stats;

        public SimpleManaAdder(HeroComponents components)
        {
            this._components = components;
            _stats = this._components.stats;
        }
        
        public void AddMana(float val)
        {
            var v = _stats.ManaCurrent.Val;
            v += val;
            _stats.ManaCurrent.SetBaseAndCurrent(v);
            if (_stats.ManaMax.Val <= v)
            {
                _stats.FullManaListener.OnFullMana(_components.gameObject);
            }
        }

        public void AddDefault() => AddMana(_stats.ManaGainPerAttack);
    }
}