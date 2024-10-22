using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/CritChanceModifierProvider", fileName = "CritChanceModifierProvider", order = 0)]
    public class CritChanceModifierProvider : ModifierProvider, IStatDecorator
    {
        [SerializeField] private float _addedPercent;
        [SerializeField] private EStatType _statType;
        
        public int order => 1;
        
        public float Decorate(float val)
        {
            return val + _addedPercent;
        }
        
        public override void AddTo(GameObject target)
        { }

        public override void AddToHero(HeroComponents components)
        {
            if (_statType == EStatType.CritChance)
            {
                components.stats.PhysicalCritChance.AddDecorator(this);
            }
            else if (_statType == EStatType.MagicCritChance)
            {
                components.stats.MagicalCritChance.AddDecorator(this);
            }
        }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return $"+{Mathf.RoundToInt(_addedPercent * 100)}% {db.descriptions[_id].parts[0]}";
        }

 
    }
}