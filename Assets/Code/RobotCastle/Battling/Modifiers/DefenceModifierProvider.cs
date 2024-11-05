using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/DefenceModifierProvider", fileName = "DefenceModifierProvider", order = 0)]
    public class DefenceModifierProvider : ModifierProvider, IStatDecorator
    {
        [SerializeField] private int _addedResist;
        
        public int order => 1;
        
        public float Decorate(float val)
        {
            return val + _addedResist;
        }
        
        public override void AddTo(GameObject target)
        { }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.PhysicalCritChance.AddDecorator(this);

        }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return $"+{_addedResist} {db.descriptions[_id].parts[0]}";
        }
    }
}