﻿using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Soul Link", fileName = "soul_link", order = 0)]
    public class SpellProviderSoulLink : SpellProvider
    {
        [SerializeField] private SpellConfigSoulLink _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents components) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}