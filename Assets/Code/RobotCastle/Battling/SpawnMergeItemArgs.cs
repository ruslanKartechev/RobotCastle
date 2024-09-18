using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpawnMergeItemArgs
    {
        public CoreItemData coreData;
        public Vector2Int preferredCoordinated;
        public bool usePreferred;

        public SpawnMergeItemArgs(CoreItemData coreData)
        {
            this.coreData = coreData;
            preferredCoordinated = default;
            usePreferred = false;
        }
        
        public SpawnMergeItemArgs(CoreItemData coreData, Vector2Int preferredCoordinated)
        {
            this.coreData = coreData;
            this.preferredCoordinated = preferredCoordinated;
            usePreferred = true;
        }
    }
}