﻿using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider IronWill", fileName = "spell_iron_will", order = 0)]
    public class SpellProviderIronWill : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigIronWill _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
            view.stats.FullManaListener = new SpellIronWill(view, _config);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            var stats = target.GetComponent<HeroStatsManager>();
            str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{stats.SpellPower.Get()}</color>");
            return str;
        }
    }
}