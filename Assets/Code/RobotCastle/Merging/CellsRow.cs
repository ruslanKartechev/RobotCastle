using System.Collections.Generic;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class CellsRow
    {
        public List<Cell> cells;
        
        public CellsRow(){}

        public CellsRow(CellsRow other)
        {
            var count = other.cells.Count;
            cells = new List<Cell>(count);
            for (var i = 0; i < count; i++)
            {
                cells.Add(new Cell(other.cells[i]));
            }
            
        }
        
        public int Count => cells.Count;

        public int CalculateItemsCount()
        {
            var count = 0;
            foreach (var cell in cells)
            {
                if (cell.isOccupied && cell.isUnlocked)
                {
                    var item = cell.currentItem;
                    if (item.pivotX == cell.x && item.pivotY == cell.y)
                        count++;
                }
            }
            return count;
        }
    }
}