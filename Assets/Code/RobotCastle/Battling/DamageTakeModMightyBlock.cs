using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling
{
    public class DamageTakeModMightyBlock : IDamageTakenModifiers, IRecurringModificator
    {
        public DamageTakeModMightyBlock(HeroComponents components, int count)
        {
            _components = components;
            _count = count;
            _maxCount = count;
        }

        public int priority { get; } = 1;
        
        public bool permanent => false;

        public HeroDamageArgs Apply(HeroDamageArgs damageInput)
        {
            CLog.Log($"Mighty block. Ignoring damage");
            _count--;
            damageInput.amount = 0;
            if (_count <= 0)
                _components.healthManager.RemoveModifier(this);
            
            ServiceLocator.Get<IDamageDisplay>().ShowMightyBlock(_components.pointMightyBlock.position);
            return damageInput;
        }
        
        public void Activate()
        {
            _count = _maxCount;
            _components.healthManager.AddModifier(this);
        }

        public void Deactivate()
        {
            _components.healthManager.RemoveModifier(this);
        }
        
        private int _count;
        private int _maxCount;
        private HeroComponents _components;
        
    }
}