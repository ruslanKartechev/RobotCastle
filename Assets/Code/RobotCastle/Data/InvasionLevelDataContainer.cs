using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Data
{
    [CreateAssetMenu(menuName = "SO/Levels/Invasion Level", fileName = "invasion_level", order = 0)]
    public class InvasionLevelDataContainer : ScriptableObject
    {
        public InvasionLevelData data;
        
        
#if UNITY_EDITOR
        [Space(20)]
        public string e_level_str;

        [ContextMenu("Fill")]
        public void E_Fill()
        {
            var count = 20;
            data.levels = new List<InvasionRoundData>(count);
            for (var lvl = 1; lvl <= count; lvl++)
            {
                var round = new InvasionRoundData();
                round.reward = 10;
                round.enemyPreset = $"{e_level_str}_{lvl}";
                switch (lvl)
                {
                    case 4 or 9 or 14 or 19:
                        round.roundType = RoundType.Smelting;
                        break;
                    case 5 or 10 or 15:
                        round.roundType = RoundType.EliteEnemy;
                        break;
                    case 20:
                        round.roundType = RoundType.Boss;
                        break;
                    default:
                        round.roundType = RoundType.Default;
                        break;
                }
                data.levels.Add(round);
                
            }
        }
        
#endif
    }
}