using System.Collections.Generic;

namespace Bomber
{
    public class MapCellsComparer : IComparer<PathNode>
    {
        private IMap _map;

        public MapCellsComparer(IMap map)
        {
            _map = map;
        }
        //
        // public int Compare(Vector2Int x, Vector2Int y)
        // {
        //     var xComparison = x.x.CompareTo(y.x);
        //     if (xComparison != 0) return xComparison;
        //     var yComparison = x.y.CompareTo(y.y);
        //     if (yComparison != 0) return yComparison;
        //     var magnitudeComparison = x.magnitude.CompareTo(y.magnitude);
        //     if (magnitudeComparison != 0) return magnitudeComparison;
        //     return x.sqrMagnitude.CompareTo(y.sqrMagnitude);
        // }
        
        public int Compare(PathNode a, PathNode b)
        {
            var xComparison = a.Position.x.CompareTo(b.Position.x);
            if (xComparison != 0) return xComparison;
            var yComparison = a.Position.y.CompareTo(b.Position.y);
            if (yComparison != 0) return yComparison;
            return 0;
            // var magnitudeComparison = a.magnitude.CompareTo(b.magnitude);
            // if (magnitudeComparison != 0) return magnitudeComparison;
            // return a.sqrMagnitude.CompareTo(b.sqrMagnitude);
        }
    }
}