using System.Collections.Generic;
using Bomber;
using NSubstitute.ClearExtensions;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class CellsMask
    {
        public CellsMask(){}

        public CellsMask(List<Vector2Int> mask)
        {
            var count = mask.Count;
            this.mask = new List<Vector2Int>(mask.Capacity);
            for (var i = 0; i < count; i++)
            {
                this.mask.Add(mask[i]);
            }
        }
        
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

        public void SetAsRotated(CellsMask srsMask, Quaternion rotation)
        {
            var cos = 1;
            var sin = 0;
            var angle = 360 - rotation.eulerAngles.y;
            if (Approximately(angle, 0))
            {
                cos = 1;
                sin = 0;
            }            
            else if (Approximately(angle, 270))
            {
                cos = 0;
                sin = -1;
            }
            else if (Approximately(angle, 180))
            {
                cos = -1;
                sin = 0;
            }
            else if (Approximately(angle, 90))
            {
                cos = 0;
                sin = 1;
            }
            mask.Clear();
            foreach (var dirCell in srsMask.mask)
            {
                var x = dirCell.x * cos - dirCell.y * sin;
                var y = dirCell.x * sin + dirCell.y * cos;
                var newVec = new Vector2Int(x, y);
                mask.Add(newVec);
            }

            bool Approximately(float input, float srcAngle) => Mathf.Abs(input - srcAngle) < 5;
        }
        
    }
}