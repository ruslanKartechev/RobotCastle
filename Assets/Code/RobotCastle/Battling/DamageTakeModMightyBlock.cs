using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling
{
    public class DamageTakeModMightyBlock : IRecurringModificator // IDamageTakenModifiers
    {
        public DamageTakeModMightyBlock(HeroComponents components, int count)
        {
            _components = components;
            _count = count;
            _maxCount = count;
        }

        public int priority { get; } = 1;
        
        // public HeroDamageArgs Apply(HeroDamageArgs damageInput)
        // {
        //     CLog.Log($"Mighty block. Ignoring damage");
        //     _count--;
        //     if (_count <= 0)
        //         _components.healthManager.RemoveModifier(this);
        //     
        //     ServiceLocator.Get<IDamageDisplay>().ShowMightyBlock(_components.pointMightyBlock.position);
        //     return damageInput;
        // }
        
        public void Activate()
        {
            _count = _maxCount;
            _components.stats.MightyBlock.Val += _maxCount;
            // _components.healthManager.AddModifier(this);
        }

        public void Deactivate()
        {
            // _components.healthManager.RemoveModifier(this);
        }
        
        private int _count;
        private int _maxCount;
        private HeroComponents _components;
        
    }
}