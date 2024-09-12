using SleepDev;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class Stat : IFloatGetter
    {
        public float BaseVal;
        public float Val;

        public void SetBaseAndCurrent(float val)
        {
            BaseVal = Val = val;
        }

        public float Get() => Val;
    }
    
}