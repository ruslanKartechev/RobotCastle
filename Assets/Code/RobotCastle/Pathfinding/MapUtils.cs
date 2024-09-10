using System.Collections.Generic;
using UnityEngine;

namespace Bomber
{
    public static class MapUtils
    {
        public static MapCell GetAt(this MapCell[,] grid, int x, int y)
        {
            return grid[x, y];
        }

        public static float DistanceEstimate2(this Vector2Int vec)
        {
            return (vec.x * vec.x + vec.y * vec.y);
        }
        
        public static float DistanceEstimateSum(this Vector2Int vec)
        {
            return (Mathf.Abs(vec.x) + Mathf.Abs(vec.y));
        }
        
        public static List<Vector2Int> GetNeighbours(Vector2Int size, Vector2Int center)
        {
            var res = new List<Vector2Int>(8);
            if (center.x > 0 && center.x < size.x - 1 
                             && center.y > 0 && center.y < size.y - 1)
            {
                res.Add(new Vector2Int(center.x - 1, center.y + 1));
                res.Add(new Vector2Int(center.x, center.y + 1));
                res.Add(new Vector2Int(center.x + 1, center.y + 1));
                res.Add(new Vector2Int(center.x - 1, center.y));
                res.Add(new Vector2Int(center.x + 1, center.y));
                res.Add(new Vector2Int(center.x - 1, center.y - 1));
                res.Add(new Vector2Int(center.x, center.y - 1));
                res.Add(new Vector2Int(center.x + 1, center.y - 1));

            }
            else
            {
                var p = new Vector2Int(center.x - 1, center.y + 1);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x, center.y + 1);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x + 1, center.y + 1);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x - 1, center.y);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x + 1, center.y);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x - 1, center.y - 1);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x, center.y - 1);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);

                p = new Vector2Int(center.x + 1, center.y - 1);
                if (p.x >= 0 && p.x <= size.x - 1 && p.y >= 0 && p.y <= size.y - 1)
                    res.Add(p);
            }
        
            return res;
        }
    }
}