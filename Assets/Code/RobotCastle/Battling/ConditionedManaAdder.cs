namespace RobotCastle.Battling
{
    public class ConditionedManaAdder : IManaAdder
    {
        /// <summary>
        /// True by default
        /// </summary>
        public bool CanAdd
        {
            get => _canAdd;
            set => _canAdd = value;
        }
        
        
        private HeroComponents _components;
        private HeroStatsManager _stats;
        private bool _canAdd = true;

        public ConditionedManaAdder(HeroComponents components)
        {
            _components = components;
            _stats = _components.stats;
        }

        public void AddMana(float val)
        {
            if (!_canAdd)
                return;
            var v = _stats.ManaCurrent.Val;
            v += val;
            _stats.ManaCurrent.SetBaseAndCurrent(v);
            if (_stats.ManaMax.Val <= v)
                _stats.FullManaListener.OnFullMana(_components.gameObject);
        }
        
        public void AddDefault() => AddMana(_stats.ManaGainPerAttack);
    }
}