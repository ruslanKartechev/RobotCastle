using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.InvasionMode
{
    [CreateAssetMenu(menuName = "SO/Difficulty Tiers Database", fileName = "Difficulty Tiers Database", order = -250)]
    public class DifficultyTiersDatabase : ScriptableObject
    {
        public List<DifficultyTierConfig> tiersConfig => _tiersConfig;
        
        [SerializeField] private List<DifficultyTierConfig> _tiersConfig;

#if UNITY_EDITOR
        
        [ContextMenu("E_Generate")]
        public void E_Generate()
        {
            foreach (var config in _tiersConfig)
            {
                var msg = "";
                if (config.enemyForcesPercent > 0)
                {
                    msg += $"+{RedInt(Mathf.RoundToInt(config.enemyForcesPercent * 100))}% Enemy Forces";
                }

                if (config.enemyTier > 0)
                {
                    msg += $"\n+{RedInt(config.enemyTier)} Enemy Tier";
                }
                
                if (config.challenngePhaseEliteEnemies > 0)
                {
                    var en = config.challenngePhaseEliteEnemies > 1 ? "Enemies" : "Enemy";
                    msg += $"\n+ Challenge phase {RedInt(config.challenngePhaseEliteEnemies)} Elite {en}";
                }

                if (config.castleDurabilityPenalty > 0)
                {
                    msg += $"\n- {RedInt(config.castleDurabilityPenalty)} Castle Durability";
                }

                if (config.additionalEliteEnemies > 0)
                {
                    var en = config.additionalEliteEnemies > 1 ? "Enemies" : "Enemy";
                    msg += $"\n{RedInt(config.castleDurabilityPenalty)} additional Elite {en}";
                }
                config.textDescription = msg;
            }

            UnityEditor.EditorUtility.SetDirty(this);
            string RedInt(int num)
            {
                return $"<color=#FF1111>{num}</color>";
            }            
            string Red(string msg)
            {
                return $"<color=#FF1111>{msg}</color>";
            }
        }
        #endif
    }
}