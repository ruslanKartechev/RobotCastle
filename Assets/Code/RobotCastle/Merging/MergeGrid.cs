using System.Collections.Generic;

namespace RobotCastle.Merging
{
    [System.Serializable]
    public class MergeGrid
    {
        public List<CellsRow> rows;

        public MergeGrid(){}

        public MergeGrid(MergeGrid other)
        {
            var count = other.rows.Count;
            rows = new List<CellsRow>(count);
            for (var i = 0; i < count; i++)
            {
                rows.Add(new CellsRow(other.rows[i]));
            }

        }
        
        
        public int RowsCount => rows.Count;

        public int CalculateTotalCellsCount()
        {
            var count = 0;
            foreach (var row in rows)
                count += row.Count;
            return count;
        }

        public int TotalItemsCount()
        {
            var count = 0;
            foreach (var row in rows)
                count += row.CalculateItemsCount();
            return count;
        }

        
        public string GetStateAsStr()
        {
            var msg = "";
            var yCount = rows.Count;
            var xCount = rows[0].Count;
            msg += $"GridSize: {xCount}_{yCount}\n";
            for (var y = 0; y < yCount; y++)
            {
                for (var x = 0; x < xCount; x++)
                {
                    var cell = rows[y].cells[x];
                    var itemStr = "";
                    if (!cell.isOccupied)
                        itemStr = "Empty";
                    else
                    {
                        itemStr = cell.currentItem.GetStr();
                    }
                    msg += $"Cell {x}_{y}. Item {itemStr}\n";
                }
            }
            return msg;
        }
    }
}