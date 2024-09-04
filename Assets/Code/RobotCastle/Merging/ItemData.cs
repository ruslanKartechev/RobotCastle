using UnityEngine;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class ItemData
    {
        public static ItemData Null => new ItemData(-1, default);

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

        public static bool operator ==(ItemData lhs, ItemData rhs)
        {
            if (lhs is null && rhs is null)
                return true;
            if (lhs is null)
                return false;
            if (rhs is null)
                return false;
            
            var equal = true;
            equal &= lhs.core == rhs.core;
            equal &= lhs.size == rhs.size;
            return equal;
        }

        public static bool operator !=(ItemData lhs, ItemData rhs)
        {
            return !(lhs == rhs);
        }

        public string GetStr() => core.ItemDataStr();
    }
}