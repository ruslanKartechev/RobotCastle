using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class Stat : IFloatGetter
    {
        public readonly List<IStatDecorator> Decorators = new List<IStatDecorator>(10);
        public readonly List<IStatDecorator> PermanentDecorators = new List<IStatDecorator>(10);

        public float BaseVal;
        public float Val;

        public void SetBaseAndCurrent(float val)
        {
            BaseVal = Val = val;
        }

        public float Get()
        {
            var v = Val;
            foreach (var dec in PermanentDecorators)
                v = dec.Decorate(v);
            foreach (var dec in Decorators)
                v = dec.Decorate(v);
            return v;
        }

        public void ClearDecorators()
        {
            Decorators.Clear();
        }
        
    }
    
}