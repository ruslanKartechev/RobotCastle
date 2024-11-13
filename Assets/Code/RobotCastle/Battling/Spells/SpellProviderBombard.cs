﻿using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/bombard", fileName = "bombard", order = 202)]
    public class SpellProviderBombard : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellBombard(_config, components);
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigBombard _config;

    }
}