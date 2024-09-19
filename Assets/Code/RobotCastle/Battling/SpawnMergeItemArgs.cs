using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpawnMergeItemArgs
    {
        public CoreItemData coreData;
        public Vector2Int preferredCoordinated;
        public bool usePreferredCoordinate;

        public ItemData ItemData { get; set; }
        public List<CoreItemData> additionalItems;
        public bool useAdditionalItems;

        public SpawnMergeItemArgs(CoreItemData coreData)
        {
            this.coreData = coreData;
            preferredCoordinated = default;
            usePreferredCoordinate = false;
            ItemData = new ItemData(coreData);
            additionalItems = null;
        }
        
        public SpawnMergeItemArgs(CoreItemData coreData, Vector2Int preferredCoordinated)
        {
            this.coreData = coreData;
            this.preferredCoordinated = preferredCoordinated;
            usePreferredCoordinate = true;
        }
    }
}