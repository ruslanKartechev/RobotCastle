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
        
        
        private HeroComponents _heroView;
        private HeroStatsManager _stats;
        private bool _canAdd = true;

        public ConditionedManaAdder(HeroComponents heroView)
        {
            _heroView = heroView;
            _stats = _heroView.stats;
        }

        public void AddMana(float val)
        {
            if (!_canAdd)
                return;
            var v = _stats.ManaCurrent.Val;
            v += val;
            _stats.ManaCurrent.SetBaseAndCurrent(v);
            if (_stats.ManaMax.Val <= v)
            {
                _stats.FullManaListener.OnFullMana(_heroView.gameObject);
            }
        }
    }
}