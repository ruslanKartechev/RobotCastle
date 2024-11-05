using RobotCastle.Core;
using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/MightyBlock ModifierProvider", fileName = "MightyBlock ModifierProvider", order = 50)]
    public class MightyBlockModifierProvider : ModifierProvider, IStatDecorator
    {
        [SerializeField] private int _blocksCount;
        
        public int order => 20;
        
        public float Decorate(float val)
        {
            return val + _blocksCount;
        }
        
        public override void AddTo(GameObject target)
        { }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.MightyBlock.AddDecorator(this);
        }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return $"+{_blocksCount} {db.descriptions[_id].parts[0]}";
        }
    }
}