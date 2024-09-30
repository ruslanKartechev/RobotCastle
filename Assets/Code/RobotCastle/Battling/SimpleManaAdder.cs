namespace RobotCastle.Battling
{
    public class SimpleManaAdder : IManaAdder
    {
        private HeroView _heroView;
        private HeroStatsManager _stats;

        public SimpleManaAdder(HeroView heroView)
        {
            _heroView = heroView;
            _stats = _heroView.stats;
        }
        
        public void AddMana(float val)
        {
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