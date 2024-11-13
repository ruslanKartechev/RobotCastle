using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/backstab_enemy", fileName = "backstab_enemy", order = 201)]
    public class SpellProviderBackstabEnemy : SpellProvider
    {
        [SerializeField] private SpellConfigBackstab _config;

        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellBackstab(_config, components);
        }

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        public override string GetDescription(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if (components == null)
                return base.GetDescription(target);
            
            var str = base.GetDescription(target);
            // var lvl = (int)HeroesManager.GetSpellTier(components.stats.MergeTier);
            // str = str.Replace("<def>", _config.defByTier[lvl].ToString());
            return str;
        }

    }
}