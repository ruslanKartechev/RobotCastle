using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/StatsModifierProvider", fileName = "StatsModifierProvider", order = 0)]
    public class StatsModifierProvider : ModifierProvider
    {
        [SerializeField] private float _addedPercent;
        [SerializeField] private EStatType _statType;

        public EStatType StatType => _statType;

        public override void AddTo(GameObject target)
        {
            switch (_statType)
            {
                case EStatType.Attack:

                    break;
                case EStatType.AttackSpeed:

                    break;
                case EStatType.Health:

                    break;
                case EStatType.SpellPower:

                    break;
                case EStatType.Range:

                    break;
                case EStatType.MoveSpeed:

                    break;
            }        
        }

        public override string GetDescription(GameObject target)
        {
            return $"+{Mathf.RoundToInt(_addedPercent)}%";
        }
    }
}