﻿using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/CritChanceModifierProvider", fileName = "CritChanceModifierProvider", order = 0)]
    public class CritChanceModifierProvider : ModifierProvider
    {
        [SerializeField] private float _addedPercent;
        [SerializeField] private EStatType _statType;
        
        public override void AddTo(GameObject target)
        { }

        public override void AddToHero(HeroView view)
        {
        }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return $"+{Mathf.RoundToInt(_addedPercent)}% {db.descriptions[_id].parts[0]}";
        }
    }
}