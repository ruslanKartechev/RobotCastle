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

        [ContextMenu("SetHeroesLevel 10")]
        public void E_SetHeroesLevel10()
        {
            E_SetAllHeroesLevel(10);
        }

        [ContextMenu("SetHeroesLevel 0")]
        public void E_SetHeroesLevel0()
        {
            E_SetAllHeroesLevel(0);
        }
        
        private void E_SetAllHeroesLevel(int level)
        {
            var list = PlayerHeroes.heroSaves;
            foreach (var h in list)
            {
                h.level = level;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
 
        [ContextMenu("1_Set_InitialSave")]
        public void E_SetInitialSave()
        {
            E_ResetChapters();
            PlayerData.musicOn = true;
            PlayerData.soundOn = true;
            PlayerData.inventory.Reset();
            PlayerData.relics.unlockedSlotsCount = 0;
            PlayerData.relics.allRelics.Clear();
            PlayerData.globalMoney = 0;
            PlayerData.levelMoney = 0;
            PlayerData.globalHardMoney = 0;
            PlayerData.playerEnergy = 100;
            PlayerData.playerEnergyMax = 40;
            PlayerData.progression.tierLevel = 0;
            PlayerData.playerXp = 0;
            PlayerData.playerLevel = 0;
            foreach (var chapter in PlayerData.progression.chapters)
            {
                chapter.unlocked = false;
                foreach (var tt in chapter.tierData)
                {
                    tt.attemptedCount = 0;
                    tt.completedCount = 0;
                    tt.completed = false;
                    tt.unlocked = false;
                }
            }

            var chapterOne = PlayerData.progression.chapters[0];
            chapterOne.unlocked = true;
            chapterOne.tierData[0].unlocked = true;

            PlayerData.altars.pointsFree = PlayerData.altars.pointsTotal = 0;
            foreach (var save in PlayerData.altars.altars)
            {
                save.points = 0;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("2_Reset_Heroes")]
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