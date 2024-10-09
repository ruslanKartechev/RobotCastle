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
        
        public int priority => 1;
        
        public float Decorate(float val)
        {
            return val + _addedPercent;
        }
        
        public override void AddTo(GameObject target)
        { }

        public override void AddToHero(HeroView view)
        {
            if (_statType == EStatType.CritChance)
            {
                view.stats.PhysicalCritChance.AddDecorator(this);
            }
            else if (_statType == EStatType.MagicCritChance)
            {
                view.stats.MagicalCritChance.AddDecorator(this);
            }
        }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return $"+{Mathf.RoundToInt(_addedPercent * 100)}% {db.descriptions[_id].parts[0]}";
        }

 
    }
}