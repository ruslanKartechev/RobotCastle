using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Modifiers/StatsModifierProvider", fileName = "StatsModifierProvider", order = 0)]
    public class StatsModifierProvider : ModifierProvider
    {
        public float AddedPercent => _addedPercent;
        public EStatType StatType => _statType;
        
        [SerializeField] private float _addedPercent;
        [SerializeField] private EStatType _statType;
        private PercentStatDecorator _decorator;
   

        private static readonly Dictionary<EStatType, string> DescriptionMap = new()
        {
            { EStatType.Attack, "ATK "},
            { EStatType.AttackSpeed, "ATK Speed "},
            { EStatType.Health, "HP "},
            { EStatType.SpellPower, "SP "},
            { EStatType.MoveSpeed, "Speed "},
            { EStatType.PhysicalResist, "Phys DEF "},
            { EStatType.MagicResist, "Magic DEF "},
        };

        public override void AddTo(GameObject target)
        {
            AddToHero(target.GetComponent<HeroView>());
        }
        
        public override void AddToHero(HeroView view)
        {
            var stats = view.stats;
            switch (_statType)
            {
                case EStatType.Attack:
                    stats.Attack.Decorators.Add(_decorator);
                    break;
                case EStatType.AttackSpeed:
                    stats.AttackSpeed.Decorators.Add(_decorator);
                    break;
                case EStatType.Health:
                    stats.HealthMax.Decorators.Add(_decorator);
                    break;
                case EStatType.MoveSpeed:
                    stats.MoveSpeed.Decorators.Add(_decorator);
                    break;
                case EStatType.PhysicalResist:
                    stats.PhysicalResist.Decorators.Add(_decorator);
                    break;
                case EStatType.MagicResist:
                    stats.MagicalResist.Decorators.Add(_decorator);
                    break;
                case EStatType.SpellPower:
                    stats.SpellPowerDecorators.Add(_decorator);
                    break;
                default:
                    CLog.LogError($"{_statType} cannot add");
                    break;
            }        
        }

        public override string GetDescription(GameObject target)
        {
            return $"{DescriptionMap[_statType]}\n+{Mathf.RoundToInt(_addedPercent)}%";
        }
        
        
        private void OnEnable()
        {
            _decorator = new PercentStatDecorator(_addedPercent);
        }
    }
}