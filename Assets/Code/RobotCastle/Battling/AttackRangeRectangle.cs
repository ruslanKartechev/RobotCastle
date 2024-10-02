using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class AttackRangeRectangle : IAttackRange
    {
        private readonly int halfWidth;
        private readonly int halfHight;
        private readonly List<Vector2Int> _cellsMask;

        public string GetStringDescription() => $"{halfWidth * 2}x{halfHight * 2}";
        
        public AttackRangeRectangle(int halfWidth, int halfHeight)
        {
            this.halfWidth = halfWidth;
            this.halfHight = halfHeight;
            _cellsMask = new List<Vector2Int>(halfWidth * halfHeight * 4);
            for (var x = -halfWidth; x <= halfWidth; x++)
            {
                for (var y = -halfHeight; y <= halfHeight; y++)
                {
                    _cellsMask.Add(new Vector2Int(x,y));
                    // CLog.Log($"{new Vector2Int(x,y).ToString()}");
                }
            }
        }

        public List<Vector2Int> GetCellsMask()
        {
            return _cellsMask;
        }

        public List<Vector2Int> ApplyMask(Vector2Int center)
        {
            var result = new List<Vector2Int>(_cellsMask.Count);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x < 0 || coord.y < 0)
                    continue;
                result.Add(coord);
            }
            return result;
        }

        public List<Vector2Int> ApplyMask(Vector2Int center, int maxX, int maxY)
        {
            var result = new List<Vector2Int>(_cellsMask.Count);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < 0 || coord.y < 0)
                    continue;
                result.Add(coord);
            }
            return result;
        }

        /// <summary>
        /// Max Inclusive. Min Exclusive
        /// </summary>
        public List<Vector2Int> ApplyMask(Vector2Int center, int minX, int minY, int maxX, int maxY)
        {
            var result = new List<Vector2Int>(_cellsMask.Count);
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < minX || coord.y < minY)
                    continue;
                result.Add(coord);
            }
            return result;
        }

        public List<ICellView> ApplyMask(Vector2Int center, IGridView gridView)
        {
            var result = new List<ICellView>(_cellsMask.Count);
            var maxX = gridView.Grid.GetLength(0);
            var maxY = gridView.Grid.GetLength(1);
            // CLog.Log($"Max x {maxX}. MaxY {maxY}");
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < 0 || coord.y < 0)
                    continue;
                var cell = gridView.GetCell(coord.x, coord.y);
                result.Add(cell);
            }
            return result;
        }

        public List<ICellView> ApplyMask(Vector2Int center, IGridView gridView, int minX, int minY)
        {
            var result = new List<ICellView>(_cellsMask.Count);
            var maxX = gridView.Grid.GetLength(0);
            var maxY = gridView.Grid.GetLength(1);
            // CLog.Log($"Max x {maxX}. MaxY {maxY}");
            foreach (var cm in _cellsMask)
            {
                var coord = center + cm;
                if(coord.x >= maxX || coord.y >= maxY || coord.x < minX || coord.y < minY)
                    continue;
                var cell = gridView.GetCell(coord.x, coord.y);
                result.Add(cell);
            }
            return result;
        }
        
        public Vector2Int GetClosestCell(Vector2Int fromPoint, Vector2Int toPoint)
        {
            var xDiff = toPoint.x - fromPoint.x;
            var yDiff = toPoint.y - fromPoint.y;
            var signX = (int)Mathf.Sign(xDiff);
            var signY = (int)Mathf.Sign(yDiff);
            var xDiffMagn = xDiff * signX;
            var yDiffMagn = yDiff * signY;
            var stepsX = signX * (xDiffMagn - halfWidth);
            var stepsY = signY * (yDiffMagn - halfHight);
            var pos = fromPoint + new Vector2Int(stepsX, stepsY);
            CLog.Log($"From: {fromPoint}. To {toPoint}. StepsX {stepsX}. StepsY {stepsY}");
            return pos;
        }
        
    }
}