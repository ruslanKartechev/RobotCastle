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
            heroView.Stats.ManaMax.SetBaseAndCurrent(_max);
            heroView.Stats.ManaCurrent.SetBaseAndCurrent(_startVal);
        }
    }
}