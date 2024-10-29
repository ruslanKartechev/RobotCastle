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
    }
}