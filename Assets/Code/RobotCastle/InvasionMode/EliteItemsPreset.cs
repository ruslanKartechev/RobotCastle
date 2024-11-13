using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class EliteItemsPreset
    {
        public string name;
        public int itemsCountMin;
        public int itemsCountMax;
        public List<CoreItemData> itemsOptions;
        
        public EliteItemsPreset(){}

        public EliteItemsPreset(EliteItemsPreset other)
        {
            name = other.name;
            itemsCountMax = other.itemsCountMax;
            itemsCountMin = other.itemsCountMin;
            itemsOptions = new List<CoreItemData>(other.itemsOptions.Count);
            foreach (var dd in other.itemsOptions)
            {
                itemsOptions.Add(new CoreItemData(dd));
            }
        }
    }
}