using RobotCastle.Core;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.InvasionMode
{
    [CreateAssetMenu(menuName = "SO/Invasion Progression", fileName = "Invasion Progression", order = -300)]
    public class ProgressionDataBaseContainer : ScriptableObject
    {
        public ProgressionDataBase database;

        
        
        
        #if UNITY_EDITOR

        [ContextMenu("E_CalculateTotalPower")]
        public void E_CalculateTotalPower()
        {
            foreach (var chapter in database.chapters)
            {
                for (var i = 0; i < chapter.tiers.Count; i++)
                {
                    var tier = chapter.tiers[i];
                    var total = HeroesPowerCalculator.CalculateTotalPowerForEnemies(chapter.levelData, i);
                    tier.totalPower = total;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        
        [ContextMenu("E_FitLevelTypes")]
        public void E_FitLevelTypes()
        {
            for (var chapterInd = 0; chapterInd < database.chapters.Count; chapterInd++)
            {
                if (chapterInd == 0)
                    continue;
                var chapter = database.chapters[chapterInd];
                var levels = chapter.levelData.levels;
                var count = levels.Count;
                for (var i = 0; i < count; i++)
                {
                    var lvl = i + 1;
                    if (lvl is 4 or 9 or 14 or 19)
                    {
                        levels[i].roundType = RoundType.Smelting;
                    }
                    else if (lvl is 5 or 10 or 15 or 20)
                    {
                        if (lvl == count)
                        {
                            levels[i].roundType = RoundType.Boss;
                            break;
                        }
                        else
                        {
                            levels[i].roundType = RoundType.EliteEnemy;
                        }
                    }
                    else
                    {
                        levels[i].roundType = RoundType.Default;
                    }
                }

            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("E_FitPresetIds")]
        public void E_FitPresetIds()
        {
            for (var chapterInd = 0; chapterInd < database.chapters.Count; chapterInd++)
            {
                if (chapterInd == 0)
                    continue;
                var chapter = database.chapters[chapterInd];
                var levels = chapter.levelData.levels;
                var idm = chapter.id;
                var count = levels.Count;
                for (var i = 0; i < count; i++)
                {
                    levels[i].enemyPreset = $"{idm}_{i + 1}";
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        #endif
        
        // IF 10,       IF 15,      IF 20
        // 1 - normal |
        // 2 - normal |
        // 3 - norm.+rew |
        // 4 - smelting |
        // 5 - elite |
        // 6 - normal |
        // 7 - normal |
        // 8 - normal |
        // 9 - normal |
        // 10 - normal |
        // 11 - normal |
        // 12 - normal |
        // 13 - normal |
        // 14 - normal |
        // 15 - normal |
        // 16 - normal |
        // 17 - normal |
        // 18 - normal |
        // 19 - normal |
        // 20 - normal |
        
        
    }
}