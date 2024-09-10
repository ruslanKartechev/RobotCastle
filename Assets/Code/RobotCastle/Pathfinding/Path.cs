using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    public class Path
    {
        public IList<Vector2Int> points;
        public bool success;
        
        public Path(IList<Vector2Int> points, bool found)
        {
            this.success = found;
            this.points = points;
        }
    }
}