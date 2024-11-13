using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/avatar", fileName = "avatar", order = 11)]
    public class SpellProviderAvatar : SpellProvider
    {
        [SerializeField] private string _fxId;
        [SerializeField] private SpellConfigAvatar _config;

        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellAvatar(_config, components, _fxId);
        }
        
        public override string GetDescription(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if (components == null)
                return base.GetDescription(target);
            
            var str = base.GetDescription(target);
            var lvl = (int)HeroesManager.GetSpellTier(components.stats.MergeTier);
            str = str.Replace("<def>", _config.defByTier[lvl].ToString());
            str = str.Replace("<mag>", _config.spellDamage.ToString());
            return str;
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
    }
    
}