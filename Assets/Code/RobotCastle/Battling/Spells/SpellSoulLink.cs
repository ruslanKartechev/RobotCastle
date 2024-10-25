using System.Threading;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class SpellSoulLink : Spell, IFullManaListener
    {
        public SpellSoulLink(SpellConfigSoulLink config, HeroComponents components)
        {
            _config = config;
            _components = components;
            _components.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            _components.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart); 
            _components.stats.ManaAdder = _manaAdder = new ConditionedManaAdder(_components);
            _components.stats.ManaResetAfterBattle = new ManaResetSpecificVal(_config.manaMax, _config.manaStart);
        }
     
        public void OnFullMana(GameObject heroGo)
        {
            _components.stats.ManaResetAfterFull.Reset(_components);
        }
     
        private CancellationTokenSource _token;
        private SpellConfigSoulLink _config;
        private ConditionedManaAdder _manaAdder;
        private SpellParticlesOnHero _fxView;
    
    }
}