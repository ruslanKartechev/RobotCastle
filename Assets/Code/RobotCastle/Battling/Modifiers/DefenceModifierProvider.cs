using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/DefenceModifierProvider", fileName = "DefenceModifierProvider", order = 0)]
    public class DefenceModifierProvider : ModifierProvider
    {
        [SerializeField] private float _addedPercent;
        [SerializeField] private EStatType _statType;
        
        public override void AddTo(GameObject target)
        { }

        public override string GetDescription(GameObject target)
        {
            var db = ServiceLocator.Get<DescriptionsDataBase>();
            return $"+{Mathf.RoundToInt(_addedPercent)}% {db.descriptions[_id].parts[0]}";
        }
    }
}