using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Saving
{
    [CreateAssetMenu(menuName = "SO/CheatManager", fileName = "CheatManager", order = -200)]
    public class CheatManager : ScriptableObject
    {
        [Header("StartCheat")] 
        public int startMoney;
        public int startPlayerLevel;
        public bool addMergeItems = true;
        public List<ItemData> cheatMergeItems;


        public void ApplyStartCheat()
        {
            var saver = ServiceLocator.Get<IDataSaver>();
            var playerData = saver.GetData<SavePlayerData>();
            playerData.levelMoney = startMoney;
            playerData.playerLevel = startPlayerLevel;

            // if (addMergeItems)
            // {
            //     var mergeSaves = saver.GetData<MergeGrid>();
            //     foreach (var item in cheatMergeItems)
            //         MergeController.AddItemToGrid(new ItemData(item), mergeSaves);
            //
            // }
        }
    }
}