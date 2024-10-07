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
        [ContextMenu("E_SetAllXp")]
        public void E_SetAllXp()
        {
            foreach (var ss in PlayerHeroes.heroSaves)
            {
                ss.xpForNext = 5;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}