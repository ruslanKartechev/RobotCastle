using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Saving
{
    [CreateAssetMenu(menuName = "SO/SavesDataBase", fileName = "SavesDataBase", order = -190)]
    public class SavesDataBase : ScriptableObject
    {
        [Header("Initial saves to start the game")]
        public SavePlayerData PlayerData;
        [Space(10)]
        public SavePlayerHeroes PlayerHeroes;
        [Space(10)]
        public MergeGrid mergeGrid;


        #if UNITY_EDITOR
        [ContextMenu("Reset Heroes")]
        public void E_ResetHeroes()
        {
            foreach (var ss in PlayerHeroes.heroSaves)
            {
                ss.level = 0;
                ss.xp = 0;
                ss.xpForNext = 5;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
        
#if UNITY_EDITOR
        [ContextMenu("Reset Chapters")]
        public void E_ResetChapters()
        {
            for (var chapterInd = 0; chapterInd < PlayerData.progression.chapters.Count; chapterInd++)
            {
                var chapter = PlayerData.progression.chapters[chapterInd];
                if (chapterInd == 0)
                {
                    chapter.unlocked = true;
                    for (var ind = 0; ind < chapter.tierData.Count; ind++)
                    {
                        var tierData = chapter.tierData[ind];
                        if (ind == 0)
                            tierData.unlocked = true;
                        else
                            tierData.unlocked = false;
                        tierData.completed = false;
                        tierData.attemptedCount = tierData.completedCount = 0;
                    }
                }
                else
                {
                    chapter.unlocked = false;
                    foreach (var tierData in chapter.tierData)
                    {
                        tierData.completed = false;
                        tierData.unlocked = false;
                        tierData.attemptedCount = tierData.completedCount = 0;
                    }
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}