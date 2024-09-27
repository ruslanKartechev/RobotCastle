using System.Collections.Generic;
using Bomber;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class CellsMask
    {
        public List<Vector2Int> mask;

        public List<Vector2Int> GetCellsAround(Vector2Int center, IMap map)
        {
            var result = new List<Vector2Int>(mask.Count);
            foreach (var dir in mask)
            {
                var c = dir + center;
                if (map.IsOutOfBounce(c))
                    continue;
                result.Add(c);
            }
            return result;
        }
        
    }
}