using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroDamageSource
    {
        private HeroView _view;
        private List<IDamageDecorator> _decorators;

        public HeroDamageSource(HeroView view)
        {
            _view = view;
            _decorators = new(10);
        }
        
        public void AddDecorator(IDamageDecorator decorator)
        {
            CLog.Log($"[{_view.gameObject.name}] Added decorator: {decorator.id}");
            _decorators.Add(decorator);
        }

        public void RemoveDecorator(IDamageDecorator decorator)
        {
            
            _decorators.Remove(decorator);
        }        
        

        public DamageArgs GetDamage()
        {
            var amount = _view.stats.Attack.Get();
            var damageArgs = new DamageArgs(amount, 0);
            return damageArgs;
        }
    }
}