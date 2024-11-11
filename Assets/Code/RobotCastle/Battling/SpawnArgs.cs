using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpawnArgs
    {
        public CoreItemData coreData;
        public Vector2Int preferredCoordinated;
        public bool usePreferredCoordinate;

        public bool useAdditionalItems;
        public List<CoreItemData> additionalItems;
        public bool overrideSpells;
        public List<string> modifiersIds;

        public ItemData ItemData { get; set; }

        public SpawnArgs(CoreItemData coreData)
        {
            this.coreData = coreData;
            preferredCoordinated = default;
            usePreferredCoordinate = false;
            ItemData = new ItemData(coreData);
            additionalItems = null;
        }
        
        public SpawnArgs(CoreItemData coreData, Vector2Int preferredCoordinated)
        {
            this.coreData = coreData;
            this.preferredCoordinated = preferredCoordinated;
            usePreferredCoordinate = true;
        }
        
        public SpawnArgs(SpawnArgs other)
        {
            coreData = new CoreItemData(other.coreData);
            preferredCoordinated = other.preferredCoordinated;
            usePreferredCoordinate = other.usePreferredCoordinate;
            ItemData = new ItemData(coreData);
            useAdditionalItems = other.useAdditionalItems;
            var count = other.additionalItems.Count;
            additionalItems = new List<CoreItemData>(count);
            for (var i = 0; i < count; i++)
                additionalItems.Add(new CoreItemData(other.additionalItems[i]));

            overrideSpells = other.overrideSpells;
            count = other.modifiersIds.Count;
            modifiersIds = new List<string>(count);
            for (var i = 0; i < count; i++)
                modifiersIds.Add(other.modifiersIds[i]);
            
        }
    }
}