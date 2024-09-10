using System.Collections.Generic;

namespace Bomber
{
    public class MapCellsComparer : IComparer<PathNode>
    {
        public int Compare(PathNode a, PathNode b)
        {
            return b.EstimatedTotalCost.CompareTo(a.EstimatedTotalCost);
            // var xComparison = a.Position.x.CompareTo(b.Position.x);
            // if (xComparison != 0) return xComparison;
            // var yComparison = a.Position.y.CompareTo(b.Position.y);
            // if (yComparison != 0) return yComparison;
            // return 0;

        }
    }
}