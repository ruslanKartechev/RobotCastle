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
        public SaveLevelsProgress LevelsProgress;
        [Space(10)]
        public SavePlayerHeroes PlayerHeroes;
        [Space(10)]
        public MergeGrid MergeGrid;
        
    }
}