namespace RobotCastle.Battling
{
    public class ManaResetSpecificVal : IManaReset
    {
        private float _max;
        private float _startVal;


        public ManaResetSpecificVal(float max, float startVal)
        {
            _max = max;
            _startVal = startVal;
        }

        public void Reset(HeroView heroView)
        {
            heroView.stats.ManaMax.SetBaseAndCurrent(_max);
            heroView.stats.ManaCurrent.SetBaseAndCurrent(_startVal);
        }
    }
}