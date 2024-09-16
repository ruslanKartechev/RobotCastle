using UnityEngine;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class Cell 
    {
        public ItemData currentItem = ItemData.Null;
        public int x;
        public int y;
        public bool isUnlocked;
        public bool isOccupied;

        public Vector2Int Coord => new Vector2Int(x, y);
        
        public Cell(){}

        public Cell(Cell other)
        {
            currentItem = new ItemData(other.currentItem);
            x = other.x;
            y = other.y;
            isUnlocked = other.isUnlocked;
            isOccupied = other.isOccupied;
        }

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            isUnlocked = true;
            isOccupied = false;
        }

        public void SetItem(ItemData item)
        {
            this.currentItem = item;
            this.isOccupied = true;
        }

        public void SetEmpty()
        {
            this.currentItem = ItemData.Null;
            this.isOccupied = false;
        }
    }
}