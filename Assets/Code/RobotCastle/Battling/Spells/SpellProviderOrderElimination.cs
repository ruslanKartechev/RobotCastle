﻿using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider OrderElimination", fileName = "OrderElimination", order = 0)]
    public class SpellProviderOrderElimination : SpellProvider
    {
        [SerializeField] private SpellConfigElimination _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroView view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}