using RobotCastle.Data;
using UnityEngine;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class ItemData
    {
        public static ItemData Null => new ItemData(-1, "");

        public CoreItemData core;
        public int pivotX;
        public int pivotY;
        public Vector2Int size = Vector2Int.one;

        public ItemData() { }

        public ItemData(ItemData other)
        {
            this.core = new CoreItemData(other.core);
            this.pivotX = other.pivotX;
            this.pivotY = other.pivotY;
            this.size = other.size;
        }

        public ItemData(int level, string id)
        {
            this.core = new CoreItemData(level, id, "");
            size = Vector2Int.one;
        }
        
        public ItemData(int level, string id, string type)
        {
            this.core = new CoreItemData(level, id, type);
            size = Vector2Int.one;
        }

        public ItemData(CoreItemData core)
        {
            this.core = new CoreItemData(core);
            size = Vector2Int.one;
        }

        public bool IsEqualTo(ItemData rhs)
        {
            if (rhs is null)
                return false;
            var equal = true;
            equal &= this.core == rhs.core;
            equal &= this.size == rhs.size;
            return equal;
        }

        public bool IsEmpty() => core.level < 0;
        
        public string GetStr() => core.AsStr();
    }
}