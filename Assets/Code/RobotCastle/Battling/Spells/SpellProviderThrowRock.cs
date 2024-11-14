using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/throw_rock", fileName = "throw_rock", order = 209)]
    public class SpellProviderThrowRock : SpellProvider, IBehaviourProvider
    {
        [SerializeField] private SpellConfigThrowRock _config;
 
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            var hero = components.GetComponent<IHeroController>();
            if (hero == null) return;
            hero.DefaultBehaviourProvider = this;
            // components.stats.FullManaListener = new SpellBloodLust(_config, components);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            str = str.Replace("<time>", _config.stunTime.ToString());
            return str;
        }

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

        public IHeroBehaviour GetBehaviour(IHeroController hero)
        {
            return new ThrowRockBehaviour(_config.squareRange, _config.stunTime);
        }
    }
}